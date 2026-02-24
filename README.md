# CRM Corporativo

Projeto de desafio tecnico para cadastro e gestao de clientes, com API em .NET e frontend em React.

## Como rodar

### Opcao 1: Docker (recomendado)
```bash
docker-compose up --build
```

- Frontend: `http://localhost:3000`
- API: `http://localhost:8080`

### Opcao 2: Local (desenvolvimento)
Backend:
```bash
cd Backend/CRM.Corporativo.Api
dotnet run
```

Frontend:
```bash
cd Frontend
npm install
npm run dev
```

- Frontend: `http://localhost:3000`
- API (dev): `http://localhost:5012`

## ADRs (decisoes arquiteturais)

### ADR-001: Banco de dados
- **Decisao:** usar banco em memoria no backend.
- **Por que:** simplifica setup para avaliacao tecnica, reduz dependencia externa e acelera execucao local.
- **Trade-off:** dados nao persistem entre reinicios.

### ADR-002: Event Store
- **Decisao:** usar event store em memoria para registrar eventos de cliente (criacao, alteracao, exclusao).
- **Por que:** atende a auditabilidade do desafio com implementacao simples e rastreavel.
- **Trade-off:** historico de eventos nao persiste apos reinicio da aplicacao.

### ADR-003: Frontend React + TypeScript
- **Decisao:** usar React com TypeScript e consumo direto da API.
- **Por que:** entrega rapida, tipagem forte no contrato dos endpoints e manutencao mais facil.
- **Trade-off:** sem biblioteca extra de estado global para manter simplicidade.