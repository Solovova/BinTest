docker pull timescale/timescaledb:latest-pg17

docker run -d --name timescaledb -p 5433:5432 -e POSTGRES_PASSWORD=vbwqu1pa timescale/timescaledb:latest-pg17
psql -h localhost -p 5433 -U postgres -d postgres


pg_dump -U postgres -Fc binance -f d:\binance_2025_05_01_2025_07_28_full.dump


docker compose up -d

version: '3.9'
services:
  timescaledb:
    image: timescale/timescaledb:latest-pg17
    container_name: timescaledb
    ports:
      - "5433:5432"
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: vbwqu1pats
    volumes:
      - d:/pgdata:/var/lib/postgresql/data
      
docker -v
docker compose version
docker ps
docker logs timescaledb

docker compose up -d
