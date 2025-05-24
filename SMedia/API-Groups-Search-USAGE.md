# API Usage Documentation: Search Groups

## Endpoint

```
GET /api/groups/search
```

## Description

This endpoint allows users to search for groups based on their name. It returns a paginated list of groups matching the search term. The search is case-insensitive and returns groups whose names contain the search term.

## Authentication Requirements

- **Required**: Yes
- **Type**: Bearer Token
- **Header**: `Authorization: Bearer {token}`

## Request Parameters

### Path Parameters

None

### Query Parameters

| Parameter  | Type    | Required | Default | Description                                       |
|------------|---------|----------|---------|---------------------------------------------------|
| searchTerm | string  | Yes      | N/A     | The term to search for in group names             |
| page       | integer | No       | 1       | The page number to retrieve (1-based)             |
| pageSize   | integer | No       | 10      | The number of groups to return per page           |

### Request Body

None

## Response Format

### Success Response (200 OK)

```json
[
  {
    "groupId": "guid",
    "groupName": "string",
    "visibility": "string",
    "createdBy": "guid",
    "createdAt": "string (ISO datetime)",
    "image": "string (URL)",
    "memberCount": integer
  },
  ...
]
```

### Field Descriptions

#### Group Object

| Field       | Type     | Description                                           |
|-------------|----------|-------------------------------------------------------|
| groupId     | guid     | Unique identifier for the group                       |
| groupName   | string   | Name of the group                                     |
| visibility  | string   | Visibility setting ("Public" or "Private")            |
| createdBy   | guid     | ID of the user who created the group                  |
| createdAt   | string   | Date and time when the group was created (ISO format) |
| image       | string   | URL to the group's image (null if none)               |
| memberCount | integer  | Number of active members in the group                 |

### Error Responses

#### Bad Request (400)

```json
{
  "error": "Search term cannot be empty."
}
```

#### Unauthorized (401)

```json
{
  "error": "Invalid token."
}
```

#### Server Error (500)

```json
{
  "error": "An error occurred while searching for groups."
}
```

## Examples

### Example 1: Search for groups with a specific term

#### Request

```http
GET /api/groups/search?searchTerm=photo
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
[
  {
    "groupId": "5030814b-cd62-45b1-890f-be4c45dd9ceb",
    "groupName": "Photography Club",
    "visibility": "Public",
    "createdBy": "4b516857-e734-40fe-887a-b3e9e3b1392a",
    "createdAt": "2025-05-22T12:49:44.3852665",
    "image": "https://socialmediastoragebinh.blob.core.windows.net/user-uploads/1747892983644-camera.jpg",
    "memberCount": 15
  },
  {
    "groupId": "d366435a-856f-4547-896a-d00b2b9ecfc1",
    "groupName": "Street Photography",
    "visibility": "Public",
    "createdBy": "4b516857-e734-40fe-887a-b3e9e3b1392a",
    "createdAt": "2025-05-22T11:35:23.8008869",
    "image": null,
    "memberCount": 8
  }
]
```

### Example 2: Search with pagination

#### Request

```http
GET /api/groups/search?searchTerm=club&page=2&pageSize=5
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
[
  {
    "groupId": "940832e4-7d05-4e23-8616-0345fec659dc",
    "groupName": "Book Club",
    "visibility": "Public",
    "createdBy": "4b516857-e734-40fe-887a-b3e9e3b1392a",
    "createdAt": "2025-05-22T11:30:15.3984644",
    "image": null,
    "memberCount": 12
  },
  {
    "groupId": "051c91c2-9cc5-4204-bc3d-647b83634410",
    "groupName": "Film Club",
    "visibility": "Public",
    "createdBy": "4b516857-e734-40fe-887a-b3e9e3b1392a",
    "createdAt": "2025-05-22T11:27:07.5640573",
    "image": null,
    "memberCount": 7
  }
]
```

### Example 3: Search with no results

#### Request

```http
GET /api/groups/search?searchTerm=nonexistent
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
[]
```

## Implementation Notes

- The search is case-insensitive and uses a contains match (returns groups whose names contain the search term).
- Only groups that match the search term and are either public or the user is a member of will be returned.
- The results are sorted by creation date, with the newest groups appearing first.
- The response includes information about the visibility of each group and the number of active members.
- This endpoint supports pagination to manage the number of results returned.

## Best Practices

### Frontend Implementation

1. **Search UI**:
   - Implement a search input field with proper validation
   - Consider adding a small delay between keystrokes and API calls
   - Show loading indicators when the search is in progress

2. **Results Display**:
   - Display group images/avatars alongside group names for easier recognition
   - Show visibility status clearly (public/private)
   - Include member counts to give context about group size
   - Consider implementing infinite scrolling or pagination controls

3. **Empty State Handling**:
   - Show appropriate messages when no results are found
   - Provide suggestions for alternative search terms
   - Consider showing trending or popular groups as fallback

### Integration Notes

- Combine this search endpoint with filtering options for more specific searches.
- Consider implementing autocomplete functionality for better user experience.
- Cache search results on the client side to improve performance for repeated searches.
- For large applications, consider implementing server-side caching for popular search terms.
