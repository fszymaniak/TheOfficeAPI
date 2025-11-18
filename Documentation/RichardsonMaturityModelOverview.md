# Richardson API Maturity Model Implementation

This project demonstrates the implementation of APIs following Richardson's API Maturity Model, also known as the Glory of REST. The model defines four levels of REST maturity, each building upon the previous level.

## Richardson Maturity Model Overview

The Richardson Maturity Model was introduced by Leonard Richardson and later popularized by Martin Fowler. It provides a way to grade your API according to the constraints of REST, helping developers understand and implement truly RESTful services.

### The Four Levels

**Level 0: The Swamp of POX (Plain Old XML)**
- Single URI endpoint
- Single HTTP method (usually POST)
- All operations tunneled through this single endpoint
- No use of HTTP status codes
- Similar to RPC-style communication

**Level 1: Resources**
- Multiple URI endpoints representing different resources
- Still uses single HTTP method (usually POST)
- Beginning to model the problem domain in terms of resources

**Level 2: HTTP Verbs**
- Proper use of HTTP methods (GET, POST, PUT, DELETE, etc.)
- Correct use of HTTP status codes
- Resources are manipulated using appropriate HTTP verbs

**Level 3: Hypermedia Controls (HATEOAS)**
- Hypermedia as the Engine of Application State
- Responses include links to related resources and possible actions
- Self-descriptive messages
- True REST implementation

## Level 0 Implementation: The Swamp of POX

Level 0 is the most basic level where we have a single endpoint that handles all operations. This is essentially RPC (Remote Procedure Call) over HTTP.

### Characteristics of Level 0:

1. **Single Endpoint**: All requests go to one URI (e.g., `/api/service`)
2. **Single HTTP Method**: Usually POST for all operations
3. **Operation in Payload**: The actual operation is specified in the request body
4. **No HTTP Status Codes**: Usually returns 200 OK for all responses
5. **RPC-Style**: Remote procedure calls disguised as HTTP

### Example Level 0 API Structure

**Get all books:**
```json
POST /api/service
Content-Type: application/json

{
  "action": "getAllBooks"
}
```
**Get specific book:**
```json
POST /api/service
Content-Type: application/json

{
"action": "getBook",
"bookId": 123
}
```

### Our Level 0 Implementation

In this project, Level 0 is implemented through a single endpoint `/api/theOffice` that handles three actions:

- `getAllSeasons` - Retrieves all available seasons
- `getSeasonEpisodes` - Gets episodes for a specific season
- `getEpisode` - Gets details for a specific episode

**Example:**
```bash
# Get all seasons
POST /api/theOffice
{
  "action": "getAllSeasons"
}

# Get episodes from season 2
POST /api/theOffice
{
  "action": "getSeasonEpisodes",
  "season": 2
}

# Get specific episode
POST /api/theOffice
{
  "action": "getEpisode",
  "season": 2,
  "episode": 1
}
```

To use Level 0, set the environment variable:
```bash
export MATURITY_LEVEL=Level0
```

## Level 1 Implementation: Resources

Level 1 introduces the concept of resources. Instead of having a single endpoint for all operations, we now have multiple resource-based endpoints. However, we still use a single HTTP method (POST) for all operations.

### Characteristics of Level 1:

1. **Multiple Resource Endpoints**: Different URIs for different resources
2. **Single HTTP Method**: Still uses POST for all operations
3. **Resource-Based URIs**: URIs represent resources, not actions
4. **No HTTP Status Codes**: Still returns 200 OK for all responses
5. **Beginning of REST**: Start of resource-oriented design

### Example Level 1 API Structure

**Get all books:**
```json
POST /api/books
Content-Type: application/json
```

**Get specific book:**
```json
POST /api/books/123
Content-Type: application/json
```

**Get book chapters:**
```json
POST /api/books/123/chapters
Content-Type: application/json
```

### Our Level 1 Implementation

In this project, Level 1 is implemented through resource-based endpoints:

- `POST /api/seasons` - Retrieves all available seasons
- `POST /api/seasons/{seasonNumber}/episodes` - Gets episodes for a specific season
- `POST /api/seasons/{seasonNumber}/episodes/{episodeNumber}` - Gets a specific episode

**Example:**
```bash
# Get all seasons
POST /api/seasons

# Get episodes from season 2
POST /api/seasons/2/episodes

# Get specific episode (Season 2, Episode 1)
POST /api/seasons/2/episodes/1
```

### Key Differences Between Level 0 and Level 1

| Aspect | Level 0 | Level 1 |
|--------|---------|---------|
| Endpoints | Single endpoint | Multiple resource-based endpoints |
| URI Design | Action-based | Resource-based |
| Resource Identification | In request body | In URI path |
| HTTP Method | POST | POST |
| Status Codes | Always 200 OK | Always 200 OK |

### Progress from Level 0 to Level 1

The key advancement in Level 1 is the introduction of **resource-based thinking**:

- **Level 0**: "I want to perform action X"
- **Level 1**: "I want to interact with resource Y"

Level 1 begins to model your API around resources (nouns) rather than actions (verbs). This is a fundamental shift in API design philosophy and is the foundation for RESTful architecture.

To use Level 1, set the environment variable:
```bash
export MATURITY_LEVEL=Level1
```
