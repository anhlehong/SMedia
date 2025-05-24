# API Usage Documentation: Approve Group Post

## Endpoint

```
POST /api/group-posts/{groupId}/approve
```

## Description

This endpoint allows a group administrator to approve or reject a pending post in a group. Group posts may require approval before they are visible to group members, particularly in moderated groups.

## Authentication Requirements

- **Required**: Yes
- **Type**: Bearer Token
- **Header**: `Authorization: Bearer {token}`

## Request Parameters

### Path Parameters

| Parameter | Type | Required | Description                    |
|-----------|------|----------|--------------------------------|
| groupId   | guid | Yes      | The ID of the group containing the post |

### Query Parameters

None

### Request Body

```json
{
  "postId": "guid",
  "approve": true
}
```

### Field Descriptions

| Field   | Type    | Required | Description                                    |
|---------|---------|----------|------------------------------------------------|
| postId  | guid    | Yes      | The ID of the post to be approved or rejected  |
| approve | boolean | Yes      | `true` to approve the post, `false` to reject  |

## Response Format

### Success Response (200 OK)

```json
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
}
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
| isApproved          | boolean  | Approval status of the post (true if approved)       |
| isVisible           | boolean  | Visibility status of the post                        |
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

#### Not Found (404)

```json
{
  "error": "Group not found."
}
```

or

```json
{
  "error": "Post not found."
}
```

#### Bad Request (400)

```json
{
  "error": "Post is not in pending status."
}
```

or

```json
{
  "error": "Post does not belong to the specified group."
}
```

#### Unauthorized (401)

```json
{
  "error": "Invalid token: user_id is missing or invalid."
}
```

or

```json
{
  "error": "User is not an admin of the group."
}
```

#### Server Error (500)

```json
{
  "error": "An error occurred while approving the post."
}
```

## Examples

### Example 1: Approve a group post

#### Request

```http
POST /api/group-posts/3fa85f64-5717-4562-b3fc-2c963f66afa6/approve
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "postId": "7ca85f64-1234-4562-b3fc-2c963f66def7",
  "approve": true
}
```

#### Response

```json
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
  "voteCount": 0,
  "isVotedByCurrentUser": false,
  "commentCount": 0
}
```

### Example 2: Reject a group post

#### Request

```http
POST /api/group-posts/3fa85f64-5717-4562-b3fc-2c963f66afa6/approve
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "postId": "8da85f64-5678-4562-b3fc-2c963f66bbb9",
  "approve": false
}
```

#### Response

```json
{
  "postId": "8da85f64-5678-4562-b3fc-2c963f66bbb9",
  "userId": "8a1c5cda-d7b2-4361-9cde-72b3f0f84d33",
  "content": "Inappropriate content that violates group rules.",
  "postedAt": "2025-05-19T10:15:30Z",
  "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "isApproved": false,
  "isVisible": false,
  "media": [],
  "voteCount": 0,
  "isVotedByCurrentUser": false,
  "commentCount": 0
}
```

### Example 3: Attempting to approve a post as a non-admin

#### Request

```http
POST /api/group-posts/3fa85f64-5717-4562-b3fc-2c963f66afa6/approve
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "postId": "7ca85f64-1234-4562-b3fc-2c963f66def7",
  "approve": true
}
```

#### Response

```json
{
  "error": "User is not an admin of the group."
}
```

## Implementation Notes

- Only group administrators can approve or reject posts.
- When a post is approved (`approve: true`):
  - The `isApproved` flag is set to `true`
  - The `isVisible` flag is set to `true`
  - The post becomes visible to all group members in the group feed
- When a post is rejected (`approve: false`):
  - The `isApproved` flag is set to `false`
  - The `isVisible` flag is set to `false`
  - The post will not appear in the group feed
- Posts can only be approved or rejected once. After a decision has been made, the post's status is finalized.
- This endpoint is only relevant for groups that require post approval (typically moderated groups).
- The endpoint requires that the post belongs to the specified group.
- The post must be in a pending state to be approved or rejected.

## Best Practices

### Frontend Implementation

1. **Admin Interface**:
   - Create a dedicated section for pending posts in the group admin interface
   - Show clear approve/reject buttons for each pending post
   - Implement confirmation dialogs for rejections
   - Provide feedback after approval/rejection actions

2. **UI Considerations**:
   - Show post preview with all media attachments before approval
   - Consider displaying the post author's information for context
   - Implement a way to provide feedback to the poster about why a post was rejected
   - Refresh the pending posts list after approval/rejection

3. **Error Handling**:
   - Handle authentication errors by redirecting to login
   - Display appropriate error messages for all error cases
   - Provide clear feedback when the user doesn't have admin privileges

### Integration Notes

- This endpoint is typically used in conjunction with a moderation queue in the group admin interface.
- Consider implementing a notification system to alert admins when new posts are pending approval.
- For user experience, consider showing pending posts to their authors with a "pending approval" indicator.
- When implementing the post creation feature for moderated groups, make sure to indicate to users that their posts will require approval.
- Consider maintaining a history of approved/rejected posts for audit purposes.
