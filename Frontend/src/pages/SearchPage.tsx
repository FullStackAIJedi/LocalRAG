import { useState, useRef } from "react";
import type { FormEvent } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getMyDocuments, uploadDocument, askQuestion } from "../api/endpoints";
import type { AskResponse } from "../api/types";

export default function SearchPage() {
  const [question, setQuestion] = useState("");
  const [response, setResponse] = useState<AskResponse | null>(null);
  const [error, setError] = useState("");
  const [showUpload, setShowUpload] = useState(false);
  const [selectDocuments, setSelectDocuments] = useState(false);
  const [selectedDocIds, setSelectedDocIds] = useState<string[]>([]);

  const fileInputRef = useRef<HTMLInputElement>(null);
  const queryClient = useQueryClient();

  const { data: documents = [] } = useQuery({
    queryKey: ["myDocuments"],
    queryFn: getMyDocuments,
  });

  const uploadMutation = useMutation({
    mutationFn: uploadDocument,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["myDocuments"] });
      if (fileInputRef.current) fileInputRef.current.value = "";
      setShowUpload(false);
    },
    onError: (err: any) => {
      setError(err.response?.data?.error || "Upload failed.");
    },
  });

  const askMutation = useMutation({
    mutationFn: askQuestion,
    onSuccess: (data) => {
      setResponse(data);
      setError("");
    },
    onError: (err: any) => {
      setError(err.response?.data?.error || "Search failed.");
      setResponse(null);
    },
  });

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      uploadMutation.mutate(file);
    }
  };

  const handleAsk = (e: FormEvent) => {
    e.preventDefault();
    if (!question.trim()) return;

    setError("");

    const docIds =
      selectDocuments && selectedDocIds.length > 0 ? selectedDocIds : null;

    askMutation.mutate({ question, documentIds: docIds });
  };

  const toggleDocument = (id: string) => {
    setSelectedDocIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id],
    );
  };

  return (
    <div className="search-page">
      <h1>Document Search</h1>
      <form onSubmit={handleAsk} className="ask-form">
        <input
          type="text"
          value={question}
          onChange={(e) => setQuestion(e.target.value)}
          placeholder="Ask a question about your documents..."
          disabled={askMutation.isPending}
        />
        <button
          type="submit"
          disabled={askMutation.isPending || !question.trim()}
        >
          {askMutation.isPending ? "Searching..." : "Ask"}
        </button>
      </form>
      <div className="checkbox-group">
        <label>
          <input
            type="checkbox"
            checked={showUpload}
            onChange={(e) => setShowUpload(e.target.checked)}
          />
          Upload a new PDF file
        </label>
      </div>

      {showUpload && (
        <div className="upload-section">
          <input
            ref={fileInputRef}
            type="file"
            accept=".pdf"
            onChange={handleFileChange}
            disabled={uploadMutation.isPending}
          />
          {uploadMutation.isPending && (
            <div className="info">
              Uploading and indexing... This may take a few minutes.
            </div>
          )}
        </div>
      )}

      {documents.length > 0 && (
        <div className="checkbox-group">
          <label>
            <input
              type="checkbox"
              checked={selectDocuments}
              onChange={(e) => setSelectDocuments(e.target.checked)}
            />
            Search only selected documents
          </label>
        </div>
      )}

      {selectDocuments && documents.length > 0 && (
        <div className="document-selector">
          {documents.map((doc) => (
            <label key={doc.id} className="document-checkbox">
              <input
                type="checkbox"
                checked={selectedDocIds.includes(doc.id)}
                onChange={() => toggleDocument(doc.id)}
              />
              <span>{doc.fileName}</span>
              <span className="doc-status">
                {doc.status === "Completed" && `(${doc.chunkCount} chunks)`}
                {doc.status === "Processing" && "(Processing...)"}
                {doc.status === "Failed" && "(Failed)"}
              </span>
            </label>
          ))}
        </div>
      )}

      {error && <div className="error">{error}</div>}

      {response && (
        <div className="answer-section">
          <h2>Answer</h2>
          <div className="answer-text">{response.answer}</div>

          {response.sources.length > 0 && (
            <div className="sources-section">
              <h3>Sources</h3>
              {response.sources.map((source, idx) => (
                <div key={idx} className="source-item">
                  <div className="source-header">
                    <strong>{source.fileName}</strong>
                    <span className="source-score">
                      Relevance: {(source.score * 100).toFixed(1)}%
                    </span>
                  </div>
                  <div className="source-preview">{source.preview}</div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
}
