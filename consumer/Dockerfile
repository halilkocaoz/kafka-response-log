FROM golang:1.16.2-alpine AS builder

ENV GOOS linux
ENV GOARCH amd64
ENV CGO_ENABLED 1

RUN apk update && apk upgrade && \
    apk add --no-cache git

RUN apk add --no-cache git build-base

WORKDIR /build
COPY go.mod .
COPY go.sum .
RUN go mod download
COPY . .
RUN go build -tags musl -o main .

# Build the application

# Move to /dist directory as the place for resulting binary folder
WORKDIR /dist

# Copy binary from build to main folder
RUN cp /build/main .

# Command to run when starting the container
CMD ["/dist/main"]

