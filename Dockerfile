FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

COPY ./server/Kafka.Example.csproj ./Kafka.Example/
RUN dotnet restore ./Kafka.Example/Kafka.Example.csproj

COPY ./server/ ./Kafka.Example/
WORKDIR /source/Kafka.Example
RUN dotnet publish --no-restore -c Release -o /app --no-cache /restore


FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

COPY ./client ./client

COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Kafka.Example.dll"]