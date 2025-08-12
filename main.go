package main

import (
	"bytes"
	"context"
	"crypto/rand"
	"encoding/json"
	"fmt"
	"io"
	"log"
	"math/big"
	"net/http"
	"net/url"
	"os"
	"regexp"
	"strings"
	"time"
)

var (
	tenantID     string
	clientID     string
	clientSecret string
	graphScope   = "https://graph.microsoft.com/.default"
	tokenURL     string
	graphAPI     = "https://graph.microsoft.com/v1.0"
	domain       string

	httpClient *http.Client
)

type TokenRequest struct {
	CPF   string `json:"cpf,omitempty"`
	Email string `json:"email,omitempty"`
	Type  string `json:"type,omitempty"`
}

func init() {
	httpClient = &http.Client{
		Timeout: 15 * time.Second,
	}
}

func validateEnv() error {
	tenantID = os.Getenv("TENANT_ID")
	clientID = os.Getenv("CLIENT_ID")
	clientSecret = os.Getenv("CLIENT_SECRET")
	domain = os.Getenv("AZURE_DOMAIN")

	if tenantID == "" || clientID == "" || clientSecret == "" || domain == "" {
		return fmt.Errorf("missing one or more required env vars: TENANT_ID, CLIENT_ID, CLIENT_SECRET, AZURE_DOMAIN")
	}
	tokenURL = fmt.Sprintf("https://login.microsoftonline.com/%s/oauth2/v2.0/token", tenantID)
	return nil
}

func getAppToken(ctx context.Context) (string, error) {
	form := url.Values{}
	form.Set("grant_type", "client_credentials")
	form.Set("client_id", clientID)
	form.Set("client_secret", clientSecret)
	form.Set("scope", graphScope)

	req, err := http.NewRequestWithContext(ctx, "POST", tokenURL, bytes.NewBufferString(form.Encode()))
	if err != nil {
		return "", fmt.Errorf("creating token request: %w", err)
	}
	req.Header.Set("Content-Type", "application/x-www-form-urlencoded")

	resp, err := httpClient.Do(req)
	if err != nil {
		return "", fmt.Errorf("request to token endpoint failed: %w", err)
	}
	defer resp.Body.Close()

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return "", fmt.Errorf("reading token response: %w", err)
	}

	if resp.StatusCode >= 400 {
		// log body
		log.Printf("[getAppToken] token endpoint returned status %d: %s", resp.StatusCode, string(body))
		return "", fmt.Errorf("token endpoint returned status %d", resp.StatusCode)
	}

	var out struct {
		AccessToken string `json:"access_token"`
	}
	if err := json.Unmarshal(body, &out); err != nil {
		log.Printf("[getAppToken] invalid JSON from token endpoint: %s", string(body))
		return "", fmt.Errorf("invalid response from token endpoint")
	}
	if out.AccessToken == "" {
		log.Printf("[getAppToken] token response missing access_token: %s", string(body))
		return "", fmt.Errorf("token response missing access_token")
	}
	return out.AccessToken, nil
}

// getUserByUPN tries to read a user by UPN; returns (nil, nil) if 404
func getUserByUPN(ctx context.Context, appToken, upn string) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/users/%s", graphAPI, upn)
	req, err := http.NewRequestWithContext(ctx, "GET", url, nil)
	if err != nil {
		return nil, fmt.Errorf("creating getUser request: %w", err)
	}
	req.Header.Set("Authorization", "Bearer "+appToken)

	resp, err := httpClient.Do(req)
	if err != nil {
		return nil, fmt.Errorf("getUser request failed: %w", err)
	}
	defer resp.Body.Close()

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, fmt.Errorf("reading getUser response: %w", err)
	}

	if resp.StatusCode == 404 {
		return nil, nil
	}
	if resp.StatusCode >= 400 {
		log.Printf("[getUserByUPN] graph returned %d for UPN %s: %s", resp.StatusCode, upn, string(body))
		return nil, fmt.Errorf("graph returned status %d", resp.StatusCode)
	}

	var user map[string]interface{}
	if err := json.Unmarshal(body, &user); err != nil {
		log.Printf("[getUserByUPN] invalid JSON for user %s: %s", upn, string(body))
		return nil, fmt.Errorf("invalid user response")
	}
	return user, nil
}

func createUser(ctx context.Context, appToken, upn, displayName, password string) (map[string]interface{}, error) {
	url := graphAPI + "/users"
	payload := map[string]interface{}{
		"accountEnabled":    true,
		"displayName":       displayName,
		"mailNickname":      sanitizeNickname(strings.Split(upn, "@")[0]),
		"userPrincipalName": upn,
		"passwordProfile": map[string]interface{}{
			"forceChangePasswordNextSignIn": false,
			"password":                      password,
		},
	}
	b, err := json.Marshal(payload)
	if err != nil {
		return nil, fmt.Errorf("marshal create user payload: %w", err)
	}
	req, err := http.NewRequestWithContext(ctx, "POST", url, bytes.NewReader(b))
	if err != nil {
		return nil, fmt.Errorf("creating createUser request: %w", err)
	}
	req.Header.Set("Authorization", "Bearer "+appToken)
	req.Header.Set("Content-Type", "application/json")

	resp, err := httpClient.Do(req)
	if err != nil {
		return nil, fmt.Errorf("createUser request failed: %w", err)
	}
	defer resp.Body.Close()

	respBody, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, fmt.Errorf("reading createUser response: %w", err)
	}

	if resp.StatusCode >= 400 {
		log.Printf("[createUser] graph returned %d creating user %s: %s", resp.StatusCode, upn, string(respBody))
		return nil, fmt.Errorf("create user error: status %d", resp.StatusCode)
	}

	var user map[string]interface{}
	if err := json.Unmarshal(respBody, &user); err != nil {
		log.Printf("[createUser] invalid JSON create user response: %s", string(respBody))
		return nil, fmt.Errorf("invalid create user response")
	}
	return user, nil
}

func generatePassword() (string, error) {
	const chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+"
	pass := make([]byte, 16)
	for i := range pass {
		n, err := rand.Int(rand.Reader, big.NewInt(int64(len(chars))))
		if err != nil {
			return "", fmt.Errorf("random generation failed: %w", err)
		}
		pass[i] = chars[n.Int64()]
	}
	return string(pass), nil
}

func requestROPC(ctx context.Context, username, password string) (map[string]interface{}, error) {
	form := url.Values{}
	form.Set("grant_type", "password")
	form.Set("client_id", clientID)
	form.Set("client_secret", clientSecret)
	// scope: request id_token + profile;
	form.Set("scope", "openid profile offline_access")
	form.Set("username", username)
	form.Set("password", password)

	req, err := http.NewRequestWithContext(ctx, "POST", tokenURL, bytes.NewBufferString(form.Encode()))
	if err != nil {
		return nil, fmt.Errorf("creating ropc request: %w", err)
	}
	req.Header.Set("Content-Type", "application/x-www-form-urlencoded")

	resp, err := httpClient.Do(req)
	if err != nil {
		return nil, fmt.Errorf("ropc request failed: %w", err)
	}
	defer resp.Body.Close()

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, fmt.Errorf("reading ropc response: %w", err)
	}

	if resp.StatusCode >= 400 {
		// log ROPC
		log.Printf("[requestROPC] ropc returned %d for user %s: %s", resp.StatusCode, username, string(body))
		return nil, fmt.Errorf("ropc failed with status %d", resp.StatusCode)
	}

	var tok map[string]interface{}
	if err := json.Unmarshal(body, &tok); err != nil {
		log.Printf("[requestROPC] invalid JSON from ropc: %s", string(body))
		return nil, fmt.Errorf("invalid ropc response")
	}
	return tok, nil
}

func tokenHandler(w http.ResponseWriter, r *http.Request) {
	ctx := r.Context()
	var req TokenRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "bad request", http.StatusBadRequest)
		return
	}
	if req.CPF == "" && req.Email == "" && req.Type != "GUEST" {
		http.Error(w, "cpf or email or type=GUEST required", http.StatusBadRequest)
		return
	}

	// get app token
	appToken, err := getAppToken(ctx)
	if err != nil {
		// log detailed error
		log.Printf("[tokenHandler] getAppToken error: %v", err)
		http.Error(w, "internal server error", http.StatusInternalServerError)
		return
	}

	var upn string
	var displayName string
	if req.CPF != "" {
		if !isValidCPF(req.CPF) {
			w.Header().Set("Content-Type", "application/json")
			w.WriteHeader(http.StatusBadRequest)
			json.NewEncoder(w).Encode(map[string]string{
				"error": "invalid CPF",
			})
			return
		}
		upn = fmt.Sprintf("%s@%s", req.CPF, domain)
		displayName = "User " + req.CPF
	} else if req.Email != "" {
		upn = req.Email
		displayName = req.Email
	} else {
		// GUEST flow (ainda não implementado)
		upn = "guest@" + domain
		displayName = "Guest"
	}

	user, err := getUserByUPN(ctx, appToken, upn)
	if err != nil {
		log.Printf("[tokenHandler] getUserByUPN error: %v", err)
		http.Error(w, "internal server error", http.StatusInternalServerError)
		return
	}

	var password string
	if user == nil {
		// cria um usuário e faz uma chamada ROPC para cadastra-lo no AD
		password, err = generatePassword()
		if err != nil {
			log.Printf("[tokenHandler] generatePassword error: %v", err)
			http.Error(w, "internal server error", http.StatusInternalServerError)
			return
		}
		_, err := createUser(ctx, appToken, upn, displayName, password)
		if err != nil {
			log.Printf("[tokenHandler] createUser error: %v", err)
			http.Error(w, "internal server error", http.StatusInternalServerError)
			return
		}
	} else {
		// usuário existe - resetar senha e usar para login
		password, err = generatePassword()
		if err != nil {
			log.Printf("[tokenHandler] generatePassword error: %v", err)
			http.Error(w, "internal server error", http.StatusInternalServerError)
			return
		}
		err = updateUserPassword(ctx, appToken, upn, password)
		if err != nil {
			log.Printf("[tokenHandler] updateUserPassword error: %v", err)
			http.Error(w, "internal server error", http.StatusInternalServerError)
			return
		}
	}

	// ROPC para novo usuário
	tokenResp, err := requestROPC(ctx, upn, password)
	if err != nil {
		log.Printf("[tokenHandler] requestROPC error: %v", err)
		http.Error(w, "internal server error", http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(tokenResp)
}

func updateUserPassword(ctx context.Context, appToken, upn, newPassword string) error {
	url := fmt.Sprintf("%s/users/%s", graphAPI, upn)
	payload := map[string]interface{}{
		"passwordProfile": map[string]interface{}{
			"forceChangePasswordNextSignIn": false,
			"password":                      newPassword,
		},
	}
	b, _ := json.Marshal(payload)
	req, err := http.NewRequestWithContext(ctx, "PATCH", url, bytes.NewReader(b))
	if err != nil {
		return err
	}
	req.Header.Set("Authorization", "Bearer "+appToken)
	req.Header.Set("Content-Type", "application/json")

	resp, err := httpClient.Do(req)
	if err != nil {
		return err
	}
	defer resp.Body.Close()

	if resp.StatusCode >= 400 {
		body, _ := io.ReadAll(resp.Body)
		return fmt.Errorf("update password error: status %d: %s", resp.StatusCode, string(body))
	}
	return nil
}

func isValidCPF(cpf string) bool {
	// Remove tudo que não é número
	re := regexp.MustCompile(`\D`)
	cpf = re.ReplaceAllString(cpf, "")

	if len(cpf) != 11 {
		return false
	}

	// CPF com todos dígitos iguais não é válido
	invalids := []string{
		"00000000000", "11111111111", "22222222222",
		"33333333333", "44444444444", "55555555555",
		"66666666666", "77777777777", "88888888888",
		"99999999999",
	}
	for _, inv := range invalids {
		if cpf == inv {
			return false
		}
	}

	// Cálculo do primeiro dígito verificador
	sum := 0
	for i := 0; i < 9; i++ {
		num := int(cpf[i] - '0')
		sum += num * (10 - i)
	}
	d1 := 11 - (sum % 11)
	if d1 >= 10 {
		d1 = 0
	}

	// Cálculo do segundo dígito verificador
	sum = 0
	for i := 0; i < 10; i++ {
		num := int(cpf[i] - '0')
		sum += num * (11 - i)
	}
	d2 := 11 - (sum % 11)
	if d2 >= 10 {
		d2 = 0
	}

	return d1 == int(cpf[9]-'0') && d2 == int(cpf[10]-'0')
}

// Remove tudo que não seja letra ou número
func sanitizeNickname(s string) string {
	re := regexp.MustCompile(`[^a-zA-Z0-9]`)
	return re.ReplaceAllString(s, "")
}

func main() {
	if err := validateEnv(); err != nil {
		log.Fatal(err)
	}
	http.HandleFunc("/token", tokenHandler)
	addr := ":8080"
	log.Printf("starting server on http://127.0.0.1%s", addr)
	if err := http.ListenAndServe(addr, nil); err != nil {
		log.Fatalf("server failed: %v", err)
	}
}
