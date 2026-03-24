abp install-libs

cd src/HQSOFT.Order.DbMigrator && dotnet run && cd -

cd src/HQSOFT.Order.Blazor && dotnet dev-certs https -v -ep openiddict.pfx -p bbd3d277-7f8f-4cbd-ad94-2aa8a4d93de6




exit 0