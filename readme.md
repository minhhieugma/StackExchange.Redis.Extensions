### Docker
We can start the redis stack with Docker so we don't have to install it locally.
- Start the container then run the tests

```bash
docker run -d --name redis-stack -p 6379:6379 -p 8001:8001 redis/redis-stack:latest
```