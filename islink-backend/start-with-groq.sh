#!/bin/bash

# Start LinkerAI server with Groq API
# Usage: ./start-with-groq.sh

cd "$(dirname "$0")/IsLink.API"

# Set environment variables
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"
export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8"
export AI_PROVIDER="groq"  # Force Groq only
export SKIP_DB_INIT="true"
export ASPNETCORE_URLS="http://localhost:5001"

# Groq API key is already in appsettings.json, but we can also set it here if needed
# export Groq__ApiKey="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"

echo "🚀 Starting LinkerAI server with Groq API..."
echo "   Server will be available at: http://localhost:5001"
echo "   Frontend: http://localhost:5173/linkerai"
echo "   Swagger: http://localhost:5001/swagger"
echo ""
echo "Press Ctrl+C to stop the server"
echo ""

dotnet run

