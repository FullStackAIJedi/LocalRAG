# LocalRAG

A fully local, free Retrieval-Augmented Generation (RAG) application. Upload PDF files, ask natural language questions, get answers grounded in your documents with cited sources. No API keys, no cloud costs, no data leaving your machine.

## What It Does

- Register and manage user profiles
- Upload PDF documents (extracted, chunked, embedded, indexed)
- Ask questions in natural language
- Get answers from a local LLM with relevance-scored sources
- Filter search to specific documents
- Everything runs locally via Ollama

## Tech Stack

**Backend**

- ASP.NET Core 10 Web API
- Entity Framework Core 10 + Npgsql
- MediatR (CQRS)
- JWT authentication + BCrypt
- PdfPig (PDF text extraction)

**AI (Local, Free)**

- Ollama runtime
- `nomic-embed-text` - 768-dim embeddings
- `qwen3:4b` - chat/generation model

**Database**

- PostgreSQL 16
- pgvector extension with HNSW index
- All relational and vector data in one database

**Frontend**

- React 19 + TypeScript
- Vite
- TanStack Query
- React Router
- Axios

**Infrastructure**

- Docker + Docker Compose

## How It Works

1. **Upload**: PDF → text extraction (PdfPig) → chunking (1000 chars, 200 overlap) → embeddings (Ollama) → stored in PostgreSQL with pgvector
2. **Query**: question → embedding → cosine similarity search in pgvector (top 5, filtered by user) → context assembled → sent to local LLM → answer + sources returned
3. **Isolation**: every query is scoped to the authenticated user's documents only

## Prerequisites

- **.NET 10 SDK** - backend runtime
- **Node.js 20+** - frontend build
- **Docker Desktop** - runs PostgreSQL with pgvector
- **Ollama** - runs AI models locally

## Setup Steps

### 1. Install .NET 10 SDK

Download from microsoft.com/dotnet. Verify:

```
dotnet --version
```

### 2. Install Node.js 20+

Download from nodejs.org. Verify:

```
node --version
npm --version
```

### 3. Install and start Docker Desktop

Download from docker.com. Start Docker Desktop and verify:

```
docker --version
docker ps
```

### 4. Install Ollama and pull models

Download from ollama.com, install, then:

```
ollama pull nomic-embed-text
ollama pull qwen3:4b
```

Verify:

```
ollama list
```

### 5. Allow Ollama to accept Docker connections

Ollama must listen on all interfaces so the backend container can reach it.

Quit Ollama from the system tray. In PowerShell (as Administrator):

```
[System.Environment]::SetEnvironmentVariable('OLLAMA_HOST', '0.0.0.0:11434', 'User')
```

Restart Ollama. Verify:

```
netstat -ano | findstr :11434
```

Should show `0.0.0.0:11434`.

Add firewall rule (PowerShell as Administrator):

```
New-NetFirewallRule -DisplayName "Ollama Docker Access" -Direction Inbound -LocalPort 11434 -Protocol TCP -Action Allow
```

### 6. Start PostgreSQL

From the project root:

```
docker compose up -d postgres
docker compose ps
```

Container `rag-postgres` should be `Up (healthy)`.

### 7. Enable pgvector extension

```
docker exec -it rag-postgres psql -U raguser -d ragdb -c "CREATE EXTENSION IF NOT EXISTS vector;"
```

### 8. Apply database migrations

In Visual Studio: **Tools → NuGet Package Manager → Package Manager Console**, select `RagApp.Application` as default project:

```
Update-Database -StartupProject RagApp.Api
```

### 9. Run the backend

Option A - Docker:

```
docker compose up -d --build
```

Backend available at `http://localhost:5000/swagger`.

Option B - Visual Studio:
Open `RagApp.slnx`, set `RagApp.Api` as startup project, press **F5**. Backend available at `https://localhost:5001/swagger`.

### 10. Run the frontend

```
cd frontend
npm install
npm run dev
```

Open `http://localhost:5173`.

## Usage

1. Register a user at `http://localhost:5173/register`
2. Log in
3. Go to Search page
4. Check "Upload PDF" → select a text-based PDF (not scanned)
5. Wait for indexing to complete (status becomes Completed)
6. Type a question → get answer with sources
7. Optionally check "Search only in selected documents" to filter

## Verifying it Works

Check the database:

```
docker exec -it rag-postgres psql -U raguser -d ragdb
```

```sql
SELECT COUNT(*) FROM "Chunks";
SELECT "FileName", "Status", "ChunkCount" FROM "Documents";
```

Watch backend logs:

```
docker compose logs backend -f
```

## Project Structure

```
RagApp/
  RagApp.Domain/          - entities (User, Document, DocumentChunk)
  RagApp.Application/     - use cases (CQRS handlers), DbContext, DTOs
  RagApp.Infrastructure/  - Ollama services, PdfPig, chunker, JWT, BCrypt
  RagApp.Api/             - controllers, Program.cs, appsettings.json
  frontend/               - React + TypeScript client
  docker-compose.yml      - PostgreSQL + backend services
  Dockerfile              - backend container image
```

## Useful Commands

Stop everything:

```
docker compose down
```

Reset database (deletes all data):

```
docker compose down -v
docker compose up -d
```

Rebuild backend without cache:

```
docker compose down
docker compose build --no-cache backend
docker compose up -d
```

Check Ollama status:

```
ollama ps
curl http://localhost:11434/api/tags
```

## Troubleshooting

**Backend can't reach Ollama**: check `netstat -ano | findstr :11434` shows `0.0.0.0:11434`. If it shows `127.0.0.1`, set `OLLAMA_HOST=0.0.0.0:11434` and restart Ollama.

**502 Bad Gateway from Vite**: backend not running. Check `docker compose ps`. Verify `vite.config.ts` proxy target is `http://127.0.0.1:5000`.

**Password authentication failed for user "raguser"**: the PostgreSQL volume was initialized with a different password. Reset with `docker compose down -v` then `docker compose up -d`.

**"relation does not exist"**: migrations not applied. Run `Update-Database` in Package Manager Console.

**Indexing is very slow**: check `ollama ps` - if shows `100% CPU`, no GPU acceleration. Consider smaller chat model (`qwen2.5:1.5b`) or fewer chunks.

**Answer is "I could not find that answer"**: the context didn't contain the answer. Rephrase the question, ask in the same language as the document, or increase top-K in `AskQuestionHandler`.
