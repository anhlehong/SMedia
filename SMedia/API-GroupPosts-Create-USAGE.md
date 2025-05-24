# API Usage Documentation: Create Group Post

## Endpoint

```
POST /api/group-posts
```

## Description

This endpoint allows users to create a new post in a specific group. The user must be a member of the group to create posts. Depending on the group's settings, posts may require approval from a group administrator before becoming visible to other members.

## Authentication Requirements

- **Required**: Yes
- **Type**: Bearer Token
- **Header**: `Authorization: Bearer {token}`

## Request Parameters

### Path Parameters

None

### Query Parameters

None

### Request Body

```json
{
  "groupId": "guid",
  "content": "string",
  "media": [
    {
      "mediaUrl": "string (URL)",
      "mediaType": "string"
    }
  ]
}
```

### Field Descriptions

| Field     | Type      | Required | Description                                      |
|-----------|-----------|----------|--------------------------------------------------|
| groupId   | guid      | Yes      | The ID of the group to post in                   |
| content   | string    | Yes      | The text content of the post                     |
| media     | array     | No       | Array of media objects to attach to the post     |

#### Media Object

| Field     | Type   | Required | Description                                   |
|-----------|--------|----------|-----------------------------------------------|
| mediaUrl  | string | Yes      | URL to the media resource                     |
| mediaType | string | Yes      | Type of media (e.g., "image", "video")        |

## Response Format

### Success Response (201 Created)

```json
{
  "postId": "guid",
  "userId": "guid",
  "content": "string",
  "postedAt": "string (ISO datetime)",
  "groupId": "guid",
  "isApproved": false,
  "isVisible": false,
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
}
```

### Field Descriptions

#### Post Object

| Field               | Type     | Description                                          |
|---------------------|----------|------------------------------------------------------|
| postId              | guid     | Unique identifier for the new post                   |
| userId              | guid     | ID of the user who created the post                  |
| content             | string   | Text content of the post                             |
| postedAt            | string   | Date and time when the post was created (ISO format) |
| groupId             | guid     | ID of the group this post belongs to                 |
| isApproved          | boolean  | Approval status of the post, may start as false      |
| isVisible           | boolean  | Visibility status of the post, may start as false    |
| media               | array    | Array of media objects attached to the post          |
| voteCount           | integer  | Number of votes/likes on the post (initially 0)      |
| isVotedByCurrentUser| boolean  | Always false for a new post                          |
| commentCount        | integer  | Number of comments on the post (initially 0)         |

#### Media Object

| Field      | Type     | Description                                        |
|------------|----------|----------------------------------------------------|
| mediaId    | guid     | Unique identifier for the media                    |
| mediaUrl   | string   | URL to the media resource                          |
| mediaType  | string   | Type of media (e.g., "image", "video")             |
| uploadedAt | string   | Date and time when the media was uploaded          |
| uploadedBy | guid     | ID of the user who uploaded the media              |

### Error Responses

#### Bad Request (400)

```json
{
  "error": "Content cannot be empty."
}
```

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

#### Not Found (404)

```json
{
  "error": "Group not found."
}
```

#### Server Error (500)

```json
{
  "error": "An error occurred while creating the post."
}
```

## Examples

### Example 1: Create a text-only post in a group

#### Request

```http
POST /api/group-posts
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "content": "Just wanted to share some thoughts about our upcoming photography exhibition. I think we should focus on urban landscapes this time."
}
```

#### Response

```json
{
  "postId": "7ca85f64-1234-4562-b3fc-2c963f66def7",
  "userId": "9b2c5cda-e8c3-4361-9cde-72b3f0f84e44",
  "content": "Just wanted to share some thoughts about our upcoming photography exhibition. I think we should focus on urban landscapes this time.",
  "postedAt": "2025-05-23T10:30:24Z",
  "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "isApproved": false,
  "isVisible": false,
  "media": [],
  "voteCount": 0,
  "isVotedByCurrentUser": false,
  "commentCount": 0
}
```

### Example 2: Create a post with media attachments

#### Request

```http
POST /api/group-posts
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "content": "Check out some of my recent urban photography shots that could be perfect for our exhibition!",
  "media": [
    {
      "mediaUrl": "https://example.com/media/city-skyline.jpg",
      "mediaType": "image"
    },
    {
      "mediaUrl": "https://example.com/media/street-life.jpg",
      "mediaType": "image"
    }
  ]
}
```

#### Response

```json
{
  "postId": "8da85f64-5678-4562-b3fc-2c963f66bbb9",
  "userId": "9b2c5cda-e8c3-4361-9cde-72b3f0f84e44",
  "content": "Check out some of my recent urban photography shots that could be perfect for our exhibition!",
  "postedAt": "2025-05-23T10:35:12Z",
  "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "isApproved": false,
  "isVisible": false,
  "media": [
    {
      "mediaId": "1ea85f64-ab12-4562-cd34-2c963f66aaa1",
      "mediaUrl": "https://example.com/media/city-skyline.jpg",
      "mediaType": "image",
      "uploadedAt": "2025-05-23T10:35:12Z",
      "uploadedBy": "9b2c5cda-e8c3-4361-9cde-72b3f0f84e44"
    },
    {
      "mediaId": "2fa85f64-cd12-4562-ef34-2c963f66bbb2",
      "mediaUrl": "https://example.com/media/street-life.jpg",
      "mediaType": "image",
      "uploadedAt": "2025-05-23T10:35:12Z",
      "uploadedBy": "9b2c5cda-e8c3-4361-9cde-72b3f0f84e44"
    }
  ],
  "voteCount": 0,
  "isVotedByCurrentUser": false,
  "commentCount": 0
}
```

### Example 3: Attempting to post in a group where the user is not a member

#### Request

```http
POST /api/group-posts
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "groupId": "5ea85f64-9913-4562-b3fc-3g963f66bdc8",
  "content": "This is a test post in a group I'm not a member of."
}
```

#### Response

```json
{
  "error": "User is not a member of the group."
}
```

## Implementation Notes

- The endpoint requires authentication with a valid JWT token that contains the user's ID.
- Only members of the specified group can create posts in that group.
- Media URLs should point to already uploaded files. The media upload process should be handled separately before calling this endpoint.
- For moderated groups, newly created posts will have `isApproved` set to `false` and will require admin approval.
- For non-moderated groups, posts may be automatically approved and set to visible.
- The response includes a Location header with the URL to access the newly created post.
- The created post will have its `postedAt` timestamp set to the current UTC time.
- The user is automatically identified from the JWT token, so there's no need to provide the user ID in the request.

## Best Practices

### Frontend Implementation

1. **Media Handling**:
   - Implement media upload functionality separate from post creation
   - Support multiple media types and preview them before posting
   - Consider implementing size and format restrictions for uploads

2. **User Experience**:
   - Show a clear indication when a post is pending approval
   - Provide feedback on successful post creation
   - Allow users to see their own pending posts even if not yet approved
   - Implement a draft saving feature for long posts

3. **Error Handling**:
   - Validate content before submission
   - Handle authentication failures appropriately
   - Show clear error messages for all error cases

### Integration Notes

- Before creating a post with media, first upload the media files to your media storage service.
- After successful post creation, consider refreshing the group feed to include the new post (if auto-approved).
- For posts requiring approval, consider showing a message to the user explaining the approval process.
- In moderated groups, implement a notification mechanism to alert group admins about pending posts.
- The post creation UI should be hidden or disabled for users who are not members of the group.
