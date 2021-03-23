FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

COPY ./server/Kartaca.Intern/Kartaca.Intern.csproj ./Kartaca.Intern/
RUN dotnet restore ./Kartaca.Intern/Kartaca.Intern.csproj

COPY ./server/Kartaca.Intern/ ./Kartaca.Intern/
WORKDIR /source/Kartaca.Intern
RUN dotnet publish --no-restore -c Release -o /app --no-cache /restore


FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

COPY ./client ./client

COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Kartaca.Intern.dll"]