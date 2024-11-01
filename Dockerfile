FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /home/app
COPY . .
RUN dotnet restore
RUN dotnet publish ./RepositoryPatternArquictecture/RepositoryPatternArquitecture.csproj -o /publish/
WORKDIR /publish
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet","RepositoryPatternArquitecture.dll"]