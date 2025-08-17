FROM golang:1.24.5 AS build-stage

WORKDIR /app
COPY go.mod go.sum ./
RUN go mod download

COPY . .

RUN CGO_ENABLED=0 GOOS=linux go build -o  /ms-auth ./cmd/server

FROM gcr.io/distroless/base

WORKDIR /

COPY --from=build-stage /ms-auth /ms-auth
COPY .env .

EXPOSE 8080

USER nonroot:nonroot

ENTRYPOINT ["/ms-auth"]