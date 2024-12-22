FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
 
WORKDIR /CasaFinanceiroApi
COPY ["/src/Web/Web.csproj", "CasaFinanceiroApi/src/Web/"]
COPY ["/src/External/Infraestructure/Infraestructure.csproj", "CasaFinanceiroApi/src/External/Infraestructure/"]
COPY ["/src/Core/Application/Application.csproj", "CasaFinanceiroApi/src/Core/Application/"]
COPY ["/src/Core/Domain/Domain.csproj", "CasaFinanceiroApi/src/Core/Domain/"]
COPY ["/src/External/Presentation/Presentation.csproj", "CasaFinanceiroApi/src/External/Presentation/"]

RUN dotnet restore "CasaFinanceiroApi/src/Web/Web.csproj"

COPY . .

WORKDIR "/CasaFinanceiroApi/src/Web"

RUN dotnet build "./Web.csproj" -c Release -o /app/build

FROM build AS publish

RUN dotnet publish "./Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]