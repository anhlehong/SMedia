# API Usage Documentation: Get Pending Group Posts

## Endpoint

```
GET /api/group-posts/pending/{groupId}
```

## Description

This endpoint retrieves a paginated list of pending posts for a specific group that are awaiting approval. Only group administrators can access this endpoint.

## Authentication Requirements

- **Required**: Yes
- **Type**: Bearer Token
- **Header**: `Authorization: Bearer {token}`

## Request Parameters

### Path Parameters

| Parameter | Type | Required | Description                    |
|-----------|------|----------|--------------------------------|
| groupId   | guid | Yes      | The ID of the group to retrieve pending posts from |

### Query Parameters

| Parameter | Type    | Required | Default | Description                                   |
|-----------|---------|----------|---------|-----------------------------------------------|
| page      | integer | No       | 1       | The page number to retrieve (1-based)         |
| pageSize  | integer | No       | 10      | The number of posts to return per page        |

### Request Body

None

## Response Format

### Success Response (200 OK)

```json
[
  {
    "id": "guid",
    "user": {
      "id": "guid",
      "name": "string",
      "avatarUrl": "string (URL)"
    },
    "content": "string",
    "media": [
      {
        "id": "guid",
        "url": "string (URL)",
        "type": "string"
      }
    ],
    "createdAt": "string (ISO datetime)",
    "groupId": "guid"
  },
  ...
]
```

### Field Descriptions

#### Pending Post Object

| Field     | Type      | Description                                        |
|-----------|-----------|---------------------------------------------------|
| id        | guid      | Unique identifier for the post                     |
| user      | object    | Information about the user who created the post    |
| content   | string    | Text content of the post                           |
| media     | array     | Array of media objects attached to the post        |
| createdAt | string    | Date and time when the post was created (ISO format) |
| groupId   | guid      | ID of the group this post belongs to               |

#### User Object

| Field     | Type     | Description                                   |
|-----------|----------|-----------------------------------------------|
| id        | guid     | Unique identifier for the user                |
| name      | string   | Display name of the user                      |
| avatarUrl | string   | URL to the user's avatar/profile image        |

#### Media Object

| Field | Type   | Description                             |
|-------|--------|-----------------------------------------|
| id    | guid   | Unique identifier for the media         |
| url   | string | URL to the media resource               |
| type  | string | Type of media (e.g., "image", "video")  |

### Error Responses

#### Unauthorized (401)

```json
{
  "error": "Invalid token: user_id is missing or invalid."
}
```

or

```json
{
  "error": "Only group administrators can access pending posts."
}
```

#### Not Found (404)

```json
{
  "error": "Group not found."
}
```

#### Server Error (500)

```json
{
  "error": "An error occurred while retrieving pending group posts."
}
```

## Examples

### Example 1: Get first page of pending posts (default pagination)

#### Request

```http
GET /api/group-posts/pending/3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
[
  {
    "id": "7ca85f64-1234-4562-b3fc-2c963f66def7",
    "user": {
      "id": "9b2c5cda-e8c3-4361-9cde-72b3f0f84e44",
      "name": "John Doe",
      "avatarUrl": "/avatar.png"
    },
    "content": "This is a sample pending post awaiting approval. It contains some interesting ideas about our next project.",
    "media": [
      {
        "id": "1ea85f64-ab12-4562-cd34-2c963f66aaa1",
        "url": "https://via.placeholder.com/600x400.png?text=Sample+Image+1",
        "type": "image"
      }
    ],
    "createdAt": "2025-05-23T06:42:30.835Z",
    "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  },
  {
    "id": "8da85f64-5678-4562-b3fc-2c963f66bbb9",
    "user": {
      "id": "8a1c5cda-d7b2-4361-9cde-72b3f0f84d33",
      "name": "Jane Smith",
      "avatarUrl": "/avatar2.png"
    },
    "content": "Here's another pending post with a question about our upcoming event schedule.",
    "media": [],
    "createdAt": "2025-05-22T15:30:45.123Z",
    "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
]
```

### Example 2: Get specific page with custom page size

#### Request

```http
GET /api/group-posts/pending/3fa85f64-5717-4562-b3fc-2c963f66afa6?page=2&pageSize=5
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
[
  {
    "id": "5fa85f64-9101-4562-b3fc-2c963f66ccc3",
    "user": {
      "id": "7d1c5cda-f8a2-4361-9cde-72b3f0f84e22",
      "name": "Mark Johnson",
      "avatarUrl": "/avatar3.png"
    },
    "content": "An older pending post that still needs approval.",
    "media": [
      {
        "id": "2fa85f64-ab12-4562-cd34-2c963f66aad4",
        "url": "https://via.placeholder.com/600x400.png?text=Project+Document",
        "type": "document"
      }
    ],
    "createdAt": "2025-05-18T09:25:11.456Z",
    "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
]
```

## Implementation Notes

- The endpoint requires authentication with a valid JWT token that contains the user's ID.
- Only group administrators can access the list of pending posts.
- This endpoint returns posts that have not yet been approved (`isApproved = false`).
- The posts are sorted by creation date, with the newest posts appearing first.
- The endpoint supports pagination to manage groups with many pending posts.
- For moderated groups, all new posts start with pending status and require admin approval.
- This endpoint is typically used by group administrators to review and moderate content.

## Best Practices

### Frontend Implementation

1. **Admin UI**:
   - Create a separate section or tab for pending posts in the group admin interface
   - Implement approve/reject actions for each pending post
   - Show clear post author information to help in moderation decisions

2. **Pagination Control**:
   - Implement UI elements for navigating between pages of pending posts
   - Show the total number of pending posts if available
   - Consider implementing visual indicators when new pending posts arrive

3. **Post Preview**:
   - Display complete post content and media for review
   - Provide context about the post author (e.g., group membership duration)
   - Offer moderation tools directly in the pending post view

### Integration Notes

- Combine this endpoint with the approval endpoint to build a complete moderation workflow.
- Consider implementing notifications for admins when new posts are pending approval.
- For large groups with high posting volume, implement additional filtering options for pending posts.
- Remember to update the pending posts list after approving or rejecting a post.
- The user information included in the response helps admins make informed moderation decisions.
