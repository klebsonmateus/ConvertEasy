# ConvertEasy

Aplicativo simples para Windows destinado a converter e preparar arquivos de forma amigável para usuários não técnicos.

## Funcionalidades atuais e planejadas

- Conversão de vídeos para MP4 compatível, com vídeo H.264, resolução máxima de 1920×1080 sem ampliação e áudio AAC estéreo a 48 kHz. Disponível agora.
- Conversão de PDFs em imagens PNG, uma por página. Planejada.
- Processamento de múltiplos arquivos. Planejado.
- Interface simples para selecionar arquivos e acompanhar o resultado. Disponível agora.
- Progresso de conversão. A indicação de atividade já está disponível; o progresso detalhado está planejado.
- Empacotamento das dependências para distribuição. Planejado.

## Estado atual

As fases 0, 1 e 2 estão implementadas. A conversão de vídeo já pode ser testada: o MP4 é salvo na pasta `Resultado`, ao lado do vídeo original. Se o nome existir, o aplicativo usa `Nome (2).mp4`, `Nome (3).mp4` e assim por diante. A conversão de PDF permanece indisponível. Consulte [o roadmap](docs/ROADMAP.md) para as próximas fases.

## Como compilar e executar

Requer Windows e o SDK do .NET 10 (ou Visual Studio 2026 com a carga de trabalho de desenvolvimento para desktop .NET). Nesta fase, `ffmpeg` também deve estar disponível no `PATH` do computador; ele ainda não acompanha o aplicativo.

```powershell
dotnet build ConvertEasy.sln
dotnet run --project src/ConvertEasy/ConvertEasy.csproj
```

Também é possível abrir `ConvertEasy.sln` no Visual Studio e iniciar o projeto.

Para testar, selecione um arquivo `.mov` na opção **Vídeo** e clique em **Converter**. O aplicativo mostra uma indicação de atividade enquanto trabalha. Quando terminar, clique em **Abrir pasta** para ver o MP4 em `Resultado`. Se ocorrer uma falha, os detalhes técnicos são registrados em `%LOCALAPPDATA%\ConvertEasy\Logs\conversao.log`.

## Licença

MIT. Veja [LICENSE](LICENSE).
