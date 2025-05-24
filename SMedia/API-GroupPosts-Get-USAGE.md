# API Usage Documentation: Get Group Posts

## Endpoint

```
GET /api/group-posts/{groupId}
```

## Description

This endpoint retrieves a paginated list of approved posts for a specific group. Posts are sorted by their posting date (newest first). Only members of the group can access this endpoint.

## Authentication Requirements

- **Required**: Yes
- **Type**: Bearer Token
- **Header**: `Authorization: Bearer {token}`

## Request Parameters

### Path Parameters

| Parameter | Type | Required | Description                    |
|-----------|------|----------|--------------------------------|
| groupId   | guid | Yes      | The ID of the group to retrieve posts from |

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
    "postId": "guid",
    "userId": "guid",
    "content": "string",
    "postedAt": "string (ISO datetime)",
    "groupId": "guid",
    "isApproved": true,
    "isVisible": true,
    "media": [
      {
        "mediaId": "guid",
        "mediaUrl": "string (URL)",
        "mediaType": "string",
        "uploadedAt": "string (ISO datetime)",
        "uploadedBy": "guid"
      }
    ],
    "voteCount": 0,
    "isVotedByCurrentUser": false,
    "commentCount": 0
  },
  ...
]
```

### Field Descriptions

#### Post Object

| Field               | Type     | Description                                          |
|---------------------|----------|------------------------------------------------------|
| postId              | guid     | Unique identifier for the post                       |
| userId              | guid     | ID of the user who created the post                  |
| content             | string   | Text content of the post                             |
| postedAt            | string   | Date and time when the post was created (ISO format) |
| groupId             | guid     | ID of the group this post belongs to                 |
| isApproved          | boolean  | Always true for retrieved posts (approval status)    |
| isVisible           | boolean  | Always true for retrieved posts (visibility status)  |
| media               | array    | Array of media objects attached to the post          |
| voteCount           | integer  | Number of votes/likes on the post                    |
| isVotedByCurrentUser| boolean  | Whether the current user has voted on this post      |
| commentCount        | integer  | Number of comments on the post                       |

#### Media Object

| Field      | Type     | Description                                        |
|------------|----------|----------------------------------------------------|
| mediaId    | guid     | Unique identifier for the media                    |
| mediaUrl   | string   | URL to the media resource                          |
| mediaType  | string   | Type of media (e.g., "image", "video")             |
| uploadedAt | string   | Date and time when the media was uploaded          |
| uploadedBy | guid     | ID of the user who uploaded the media              |

### Error Responses

#### Unauthorized (401)

```json
{
  "error": "Invalid token."
}
```

or

```json
{
  "error": "User is not a member of the group."
}
```

#### Server Error (500)

```json
{
  "error": "An error occurred while retrieving group posts."
}
```

## Examples

### Example 1: Get first page of group posts (default pagination)

#### Request

```http
GET /api/group-posts/3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
[
  {
    "postId": "7ca85f64-1234-4562-b3fc-2c963f66def7",
    "userId": "9b2c5cda-e8c3-4361-9cde-72b3f0f84e44",
    "content": "Check out my new camera setup! Perfect for our photography club.",
    "postedAt": "2025-05-20T14:30:24Z",
    "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "isApproved": true,
    "isVisible": true,
    "media": [
      {
        "mediaId": "1ea85f64-ab12-4562-cd34-2c963f66aaa1",
        "mediaUrl": "https://example.com/media/camera-setup.jpg",
        "mediaType": "image",
        "uploadedAt": "2025-05-20T14:30:24Z",
        "uploadedBy": "9b2c5cda-e8c3-4361-9cde-72b3f0f84e44"
      }
    ],
    "voteCount": 12,
    "isVotedByCurrentUser": true,
    "commentCount": 3
  },
  {
    "postId": "8da85f64-5678-4562-b3fc-2c963f66bbb9",
    "userId": "8a1c5cda-d7b2-4361-9cde-72b3f0f84d33",
    "content": "Meeting summary from yesterday's group discussion.",
    "postedAt": "2025-05-19T10:15:30Z",
    "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "isApproved": true,
    "isVisible": true,
    "media": [],
    "voteCount": 5,
    "isVotedByCurrentUser": false,
    "commentCount": 7
  },
  // More posts...
]
```

### Example 2: Get specific page with custom page size

#### Request

```http
GET /api/group-posts/3fa85f64-5717-4562-b3fc-2c963f66afa6?page=2&pageSize=5
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
[
  {
    "postId": "5fa85f64-9101-4562-b3fc-2c963f66ccc3",
    "userId": "7d1c5cda-f8a2-4361-9cde-72b3f0f84e22",
    "content": "Earlier post from last week.",
    "postedAt": "2025-05-15T08:45:12Z",
    "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "isApproved": true,
    "isVisible": true,
    "media": [
      {
        "mediaId": "2fa85f64-ab12-4562-cd34-2c963f66aad4",
        "mediaUrl": "https://example.com/media/document.pdf",
        "mediaType": "document",
        "uploadedAt": "2025-05-15T08:45:12Z",
        "uploadedBy": "7d1c5cda-f8a2-4361-9cde-72b3f0f84e22"
      }
    ],
    "voteCount": 3,
    "isVotedByCurrentUser": false,
    "commentCount": 1
  },
  // More posts...
]
```

## Implementation Notes

- The endpoint requires authentication with a valid JWT token that contains the user's ID.
- Only members of the specified group can access its posts.
- This endpoint returns only approved and visible posts.
- The posts are sorted by posting date, with the newest posts appearing first.
- The endpoint supports pagination to manage large groups with many posts.
- The response includes information about whether the current user has voted on each post.
- For each post, the number of votes and comments is included for display purposes.
- All media attachments for each post are included in the response.

## Best Practices

### Frontend Implementation

1. **Pagination Control**:
   - Implement UI elements for navigating between pages
   - Consider implementing infinite scrolling for a smoother experience
   - Show loading indicators when fetching additional pages

2. **Media Handling**:
   - Implement responsive display of media attachments
   - Support different media types (images, videos, documents)
   - Consider lazy loading for media to improve performance

3. **User Interaction**:
   - Use the `isVotedByCurrentUser` flag to display appropriate like/vote buttons
   - Implement proper rendering of post content (formatting, links, etc.)
   - Show post metadata clearly (author, date, vote count, comment count)

### Integration Notes

- This endpoint is ideal for building the main feed of group posts.
- Combine with user profile information to display the author's details with each post.
- Consider implementing real-time updates for active groups.
- For performance optimization, consider caching post data on the client side.
- When implementing the post creation feature, remember that new posts in some groups may require admin approval before appearing in this feed.
