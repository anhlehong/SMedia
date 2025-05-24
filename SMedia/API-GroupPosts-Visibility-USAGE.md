# API Usage Documentation: Update Group Post Visibility

This endpoint allows group administrators to toggle the visibility of a post within a group.

## Endpoint

```
PUT /api/group-posts/visible
```

## Description

Group administrators can use this endpoint to control the visibility of posts within their groups. Hidden posts remain in the database but are not shown to regular group members in feeds and listings.

## Authorization

- Bearer token required
- User must be an administrator of the group to which the post belongs

## Request Body

```json
{
  "postId": "string (Guid)",
  "isVisible": boolean
}
```

### Fields

- `postId`: The ID of the post to update
- `isVisible`: The new visibility state (true = visible, false = hidden)

## Responses

### 200 OK

The visibility was successfully updated.

```json
{
  "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "content": "string",
  "postedAt": "2023-06-07T12:34:56.789Z",
  "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "isApproved": true,
  "isVisible": true,
  "media": [
    {
      "mediaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "mediaUrl": "string",
      "mediaType": "string",
      "uploadedAt": "2023-06-07T12:34:56.789Z",
      "uploadedBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }
  ],
  "voteCount": 0,
  "isVotedByCurrentUser": false,
  "commentCount": 0
}
```

### Field Descriptions

| Field                | Type        | Description                                            |
|----------------------|-------------|--------------------------------------------------------|
| postId               | guid        | Unique identifier for the post                         |
| userId               | guid        | ID of the user who created the post                    |
| content              | string      | The text content of the post                           |
| postedAt             | datetime    | When the post was created (ISO 8601 format)            |
| groupId              | guid        | ID of the group the post belongs to                    |
| isApproved           | boolean     | Whether the post has been approved by an admin         |
| isVisible            | boolean     | Whether the post is currently visible to group members |
| media                | array       | Media attached to the post (images, videos, etc.)      |
| voteCount            | integer     | Number of votes/likes the post has received            |
| isVotedByCurrentUser | boolean     | Whether the current user has voted for this post       |
| commentCount         | integer     | Number of comments on the post                         |

### 400 Bad Request

Either the post is not a group post, or its visibility state is already set to the requested value.

```json
{
  "error": "string"
}
```

### 401 Unauthorized

The user is not authorized to update the post visibility (not an admin of the group).

```json
{
  "error": "string"
}
```

#### Possible Errors

- `"Invalid token: user_id is missing or invalid."`
- `"User is not an admin of the group."`

### 404 Not Found

The specified post was not found.

```json
{
  "error": "string"
}
```

#### Possible Errors

- `"Post not found."`
- `"Post not found in the group."`

### 500 Internal Server Error

An unexpected error occurred on the server.

```json
{
  "error": "string"
}
```

## Example

### Request

```http
PUT /api/group-posts/visible HTTP/1.1
Host: api.example.com
Authorization: Bearer <token>
Content-Type: application/json

{
  "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "isVisible": true
}
```

### Response

```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "content": "This is a sample post content",
  "postedAt": "2023-06-07T12:34:56.789Z",
  "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "isApproved": true,
  "isVisible": true,
  "media": [
    {
      "mediaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "mediaUrl": "https://example.com/images/sample.jpg",
      "mediaType": "image/jpeg",
      "uploadedAt": "2023-06-07T12:34:56.789Z",
      "uploadedBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }
  ],
  "voteCount": 5,
  "isVotedByCurrentUser": false,
  "commentCount": 3
}
```

### Example 2: Hiding a group post

#### Request

```http
PUT /api/group-posts/visible HTTP/1.1
Host: api.example.com
Authorization: Bearer <token>
Content-Type: application/json

{
  "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "isVisible": false
}
```

#### Response

```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "content": "This is a sample post content",
  "postedAt": "2023-06-07T12:34:56.789Z",
  "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "isApproved": true,
  "isVisible": false,
  "media": [
    {
      "mediaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "mediaUrl": "https://example.com/images/sample.jpg",
      "mediaType": "image/jpeg",
      "uploadedAt": "2023-06-07T12:34:56.789Z",
      "uploadedBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }
  ],
  "voteCount": 5,
  "isVotedByCurrentUser": false,
  "commentCount": 3
}
```

### Example 3: Trying to update visibility for a non-group post

#### Request

```http
PUT /api/group-posts/visible HTTP/1.1
Host: api.example.com
Authorization: Bearer <token>
Content-Type: application/json

{
  "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "isVisible": true
}
```

#### Response

```http
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
  "error": "This operation is only applicable to group posts."
}
```
```

## Notes

- Only group administrators can change post visibility
- Changing visibility to `false` will hide the post from regular group members, but it will still be visible to administrators
- Posts can be made visible or hidden regardless of their approval status
- When a post is hidden, it will not appear in group feeds or post listings for regular members
- This feature is useful for temporarily removing content without deleting it or rejecting it

## Best Practices

### Frontend Implementation

1. **Administrator Interface**:
   - Implement visibility toggle buttons or switches on post cards/details
   - Clearly indicate current visibility status for administrators
   - Consider showing hidden posts with a visual indicator (grayed out, badge, etc.)
   - Update the UI immediately after toggling visibility to provide instant feedback

2. **Error Handling**:
   - Display appropriate error messages based on the API responses
   - Handle cases where the post is not found or the user doesn't have permission
   - Consider implementing confirmation dialogs before hiding/showing posts

3. **User Experience**:
   - For bulk moderation, consider implementing batch visibility updates
   - When displaying posts to regular group members, filter out posts where `isVisible=false`
   - Consider implementing notifications for post creators when their posts are hidden

### Integration Notes

- Use this endpoint in conjunction with the approval process for comprehensive content moderation
- The visibility toggle complements the approval system, allowing for more nuanced control
- Consider implementing analytics to track hidden vs. visible content statistics
- Implement the visibility status in the pending posts list to help administrators manage content
