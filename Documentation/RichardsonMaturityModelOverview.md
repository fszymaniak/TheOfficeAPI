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
