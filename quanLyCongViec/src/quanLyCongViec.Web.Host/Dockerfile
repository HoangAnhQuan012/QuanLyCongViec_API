FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src
COPY ["src/quanLyCongViec.Web.Host/quanLyCongViec.Web.Host.csproj", "src/quanLyCongViec.Web.Host/"]
COPY ["src/quanLyCongViec.Web.Core/quanLyCongViec.Web.Core.csproj", "src/quanLyCongViec.Web.Core/"]
COPY ["src/quanLyCongViec.Application/quanLyCongViec.Application.csproj", "src/quanLyCongViec.Application/"]
COPY ["src/quanLyCongViec.Core/quanLyCongViec.Core.csproj", "src/quanLyCongViec.Core/"]
COPY ["src/quanLyCongViec.EntityFrameworkCore/quanLyCongViec.EntityFrameworkCore.csproj", "src/quanLyCongViec.EntityFrameworkCore/"]
WORKDIR "/src/src/quanLyCongViec.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/quanLyCongViec.Web.Host", "src/quanLyCongViec.Web.Host"]
COPY ["src/quanLyCongViec.Web.Core", "src/quanLyCongViec.Web.Core"]
COPY ["src/quanLyCongViec.Application", "src/quanLyCongViec.Application"]
COPY ["src/quanLyCongViec.Core", "src/quanLyCongViec.Core"]
COPY ["src/quanLyCongViec.EntityFrameworkCore", "src/quanLyCongViec.EntityFrameworkCore"]
WORKDIR "/src/src/quanLyCongViec.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "quanLyCongViec.Web.Host.dll"]
