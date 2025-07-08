# Create an incident

```
curl -X 'POST' \
  'https://localhost:7160/api/v1/Incident' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "severity": 2,
  "title": "string 1",
  "description": "string 1"
}'
```

# Get incidents

```
curl -X 'GET' \
  'https://localhost:7160/api/v1/Incident?SearchTerm=test&Page=1&Size=10' \
  -H 'accept: */*'
```

You can also launch the project as it's configured with Swagger to test the endpoints!