# Roadmap

- [x] **Fase 0 - Estrutura inicial do projeto.** Solução WPF em .NET, README, licença MIT e `.gitignore`.
- [x] **Fase 1 - Interface básica.** Tela inicial, opções Vídeo e PDF, seleção de um arquivo e indicação do nome escolhido. Conversão indisponível nesta fase.
- [ ] **Fase 2 — Conversão de vídeos.** Gerar MP4 com vídeo H.264/AVC, formato de pixels `yuv420p` e resolução máxima de 1920×1080; áudio AAC a 48 kHz.
- [ ] **Fase 3 — Conversão de PDF para imagens.** Gerar uma imagem PNG por página, preservando a proporção original, sem recorte ou imposição de 16:9.
- [ ] **Fase 4 — Pasta Resultado e regras de nomenclatura.** Criar `Resultado` no mesmo diretório do arquivo original; preservar o nome base sempre que possível. Exemplo: `Culto.mov` → `Resultado/Culto.mp4`; `Avisos.pdf` → `Resultado/Avisos 01.png`, `Avisos 02.png`, `Avisos 03.png`. Numerar páginas com zero à esquerda (`01`, `02`, …, `10`, `11`). Nunca sobrescrever silenciosamente um arquivo existente; definir o comportamento exato nesta fase.
- [ ] **Fase 5 — Progresso, cancelamento e tratamento de erros.** Mostrar o andamento de forma compreensível para o usuário.
- [ ] **Fase 6 — Arrastar e soltar e processamento de múltiplos arquivos.** Completar a área reservada na interface.
- [ ] **Fase 7 — Inclusão do FFmpeg e dependências no aplicativo.** Distribuir FFmpeg e FFprobe com o aplicativo, sem exigir instalação manual pelo voluntário.
- [ ] **Fase 8 — Publicação e instalador Windows.** Preparar a distribuição para uso no notebook.
- [ ] **Fase 9 — Testes no notebook utilizado na igreja.** Validar os arquivos de entrada e saída no ambiente real.
