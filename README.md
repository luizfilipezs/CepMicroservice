# CepMicroservice

This microsrevice is an API which receives a CEP via `api/address/{cep}` and returns the address. It consumes the third party [Correios API](https://viacep.com.br) and caches the results inside a SQLite database.

The idea behind this project is to learn C# and .NET with a simple but real case like microservice.

## Running

```bash
dotnet run
```

> Before running this and the other commands, run `dotnet build` in case that you did not compile the code yet or if you make any changes.

## Testing

### Unit tests

```bash
dotnet test --filter "TestCategory=Unit"
```

## Integration tests

```bash
dotnet test --filter "TestCategory=Integration"
```

### Both

```bash
dotnet test
```
