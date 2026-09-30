# Conversor Holyrics

Aplicativo Windows para ajudar voluntários a preparar arquivos para uso no Holyrics, com uma interface simples e sem exigir conhecimentos técnicos. O projeto atende à necessidade de converter vídeos incompatíveis e apresentações em PDF antes de usá-los no computador da igreja.

## Funcionalidades planejadas

- Converter vídeos para MP4 compatível com o computador e o Holyrics.
- Transformar cada página de um PDF em uma imagem PNG.

## Estado atual

As fases 0 e 1 estão implementadas: estrutura do projeto e interface básica. É possível escolher um arquivo de vídeo ou PDF e ver seu nome na tela. Os botões de conversão estão desabilitados. O processamento, a pasta de saída, o arrastar e soltar e a distribuição para usuários finais serão desenvolvidos nas próximas fases. Consulte [o roadmap](docs/ROADMAP.md).

## Como compilar e executar

Requer Windows e o SDK do .NET 10 (ou Visual Studio 2026 com a carga de trabalho de desenvolvimento para desktop .NET).

```powershell
dotnet build ConversorHolyrics.sln
dotnet run --project src/ConversorHolyrics/ConversorHolyrics.csproj
```

Também é possível abrir `ConversorHolyrics.sln` no Visual Studio e iniciar o projeto.

## Licença

MIT. Veja [LICENSE](LICENSE).
