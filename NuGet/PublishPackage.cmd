echo "Create Package"
..\src\.nuget\nuget pack  ..\src\Autofac.TypedFactories\Autofac.TypedFactories.csproj
echo "Create Symbol Package"
..\src\.nuget\nuget pack  ..\src\Autofac.TypedFactories\Autofac.TypedFactories.csproj -symbols