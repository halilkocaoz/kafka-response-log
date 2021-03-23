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



WORKDIR /dist
RUN cp /build/main .
CMD ["/dist/main"]