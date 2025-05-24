# API Usage Documentation: Get Pending Group Posts

## Endpoint

```
GET /api/group-posts/pending/{groupId}
```

## Description

This endpoint retrieves a list of pending posts that are visible for a specific group. Only group administrators can access this endpoint. The endpoint filters to include only posts that are not approved (pending) and are marked as visible.

## Authentication Requirements

- **Required**: Yes
- **Type**: Bearer Token
- **Header**: `Authorization: Bearer {token}`
- **Access Level**: Group administrators only

## Request Parameters

### Path Parameters

| Parameter | Type | Required | Description                        |
|-----------|------|----------|------------------------------------|
| groupId   | guid | Yes      | The ID of the group to query       |

### Query Parameters

| Parameter | Type    | Required | Default | Description                                |
|-----------|---------|----------|---------|--------------------------------------------|
| page      | integer | No       | 1       | The page number for pagination             |
| pageSize  | integer | No       | 10      | The number of posts to return per page     |

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
      "avatarUrl": "string"
    },
    "content": "string",
    "media": [
      {
        "id": "guid",
        "url": "string",
        "type": "string"
      }
    ],
    "createdAt": "string (ISO datetime)",
    "groupId": "guid",
    "isVisible": boolean
  }
]
```

### Field Descriptions

#### Pending Post Object

| Field     | Type       | Description                                               |
|-----------|------------|-----------------------------------------------------------|
| id        | guid       | Unique identifier for the post                            |
| user      | UserObject | Information about the post creator                        |
| content   | string     | The text content of the post                              |
| media     | array      | Array of media (images, videos) attached to the post      |
| createdAt | string     | Date and time when the post was created (ISO format)      |
| groupId   | guid       | The ID of the group the post belongs to                   |
| isVisible | boolean    | Whether the post is currently visible in the group or not |

#### User Object

| Field     | Type   | Description                                |
|-----------|--------|--------------------------------------------|
| id        | guid   | Unique identifier for the user             |
| name      | string | Display name of the user                   |
| avatarUrl | string | URL to the user's profile picture          |

#### Media Object

| Field | Type   | Description                                 |
|-------|--------|---------------------------------------------|
| id    | guid   | Unique identifier for the media             |
| url   | string | URL to the media file                       |
| type  | string | Media type (e.g., "image", "video")         |

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

### Example 1: Get pending posts for a group

#### Request

```http
GET /api/group-posts/pending/b529b743-a897-4abe-be9b-aa495482d969?page=1&pageSize=5
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
[
  {
    "id": "44885529-7ac9-4131-916c-d10791cd1973",
    "user": {
      "id": "141f1925-9f60-4316-ba29-ca27ed68b9ba",
      "name": "4801104003",
      "avatarUrl": "https://res.cloudinary.com/dapvvdxw7/image/upload/v1747159636/avatar_l2rwth.jpg"
    },
    "content": "Xin chào đây là test ",
    "media": [],
    "createdAt": "2025-05-24T04:37:42.055838",
    "groupId": "b529b743-a897-4abe-be9b-aa495482d969",
    "isVisible": false
  }
]
```

## Implementation Notes

- Only group administrators can access pending posts for their groups.
- Only posts that are pending (not approved) AND visible are returned.
- Hidden posts (where isVisible = false) are not included in the response.
- The `isVisible` property indicates whether the post is currently visible in the group.
- Posts are returned in reverse chronological order (newest first).
- The API supports pagination to handle large numbers of pending posts.
- Media attachments, if any, are included in the response for each post.
- The response provides user information for each post creator, including their name and avatar URL.

## Best Practices

### Frontend Implementation

1. **Administrator UI**:
   - Display pending posts in a moderation queue interface
   - Clearly show the post content, attached media, and creator information
   - Provide approve/reject buttons for each post
   - Show the post visibility status and allow toggling it

2. **Error Handling**:
   - Handle the case where a user is not an administrator
   - Display appropriate messages when no pending posts are found
   - Handle pagination appropriately

### Integration Notes

- Use this endpoint in conjunction with the approval endpoint (`POST /api/group-posts/{groupId}/approve`) to moderate group content.
- The visibility toggle endpoint (`PUT /api/group-posts/visible`) can be used to change the visibility of posts without changing their approval status.
- Consider implementing a notification system to alert administrators when new pending posts are available for review.
