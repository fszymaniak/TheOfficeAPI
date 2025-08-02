# The Office API - Richardson Maturity Level 0

This project implements The Office TV series API starting with Richardson Maturity Level 0 (The Swamp of POX).

## Level 0: The Swamp of POX

In Level 0, we use:
- **Single endpoint**: All requests go to `/api/service`
- **Single HTTP method**: POST for all operations
- **Operation in payload**: The action is specified in the request body
- **No proper HTTP status codes**: Always returns 200 OK
- **RPC-style communication**: Remote procedure calls over HTTP

## API Operations

All operations use the same endpoint: `POST /api/service`

### Get All Seasons
```json
POST /api/service
Content-Type: application/json

{
  "action": "getAllSeasons"
}
```
### Get Episodes from Season
```json
POST /api/service
Content-Type: application/json

{
"action": "getSeasonEpisodes",
"season": "01"
}
```
### Get Single Episode
```json
POST /api/service
Content-Type: application/json

{
  "action": "getEpisode",
  "season": "01",
  "episode": "01"
}
```
## Response Format
All responses return HTTP 200 OK with success/error information in the body:
```json
{
"success": true,
"data": { ... },
"message": "Operation completed successfully"
}
```
## Example Responses
### All Seasons Response
```json
{
"success": true,
"data": [
{ "season": "01", "episodeCount": 6 },
{ "season": "02", "episodeCount": 22 },
{ "season": "03", "episodeCount": 25 }
],
"message": "Seasons retrieved successfully"
}
```
### Season Episodes Response
```json
{
  "success": true,
  "data": [
    {
      "season": "01",
      "episodeNumber": "01",
      "title": "Pilot",
      "releasedDate": "2005-03-24"
    },
    {
      "season": "01",
      "episodeNumber": "02",
      "title": "Diversity Day",
      "releasedDate": "2005-03-29"
    }
  ],
  "message": "Episodes retrieved successfully"
}
```
### Single Episode Response
```json
{
  "success": true,
  "data": {
    "season": "01",
    "episodeNumber": "01",
    "title": "Pilot",
    "releasedDate": "2005-03-24"
  },
  "message": "Episode retrieved successfully"
}
```
## Level 0 Characteristics Demonstrated

This implementation shows all the key characteristics of Richardson Maturity Level 0:

1. **Single Endpoint**: All operations go through `POST /api/service`
2. **Single HTTP Method**: Only POST is used for all operations
3. **RPC-Style**: Operations are specified in the request payload
4. **No HTTP Status Codes**: Always returns 200 OK, with success/failure in the response body
5. **Action-Based**: Uses action strings like "getAllSeasons", "getSeasonEpisodes", "getEpisode"

## Testing the API

You can test this Level 0 API using the following POST requests to `/api/service`:

1. **Get all seasons**: `{"action": "getAllSeasons"}`
2. **Get season episodes**: `{"action": "getSeasonEpisodes", "season": "01"}`
3. **Get single episode**: `{"action": "getEpisode", "season": "01", "episode": "01"}`

This represents the most basic level of REST maturity - essentially RPC over HTTP!
