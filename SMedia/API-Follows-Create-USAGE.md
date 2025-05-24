# API Usage Documentation: Follow User

## Endpoint

```
POST /api/Follows
```

## Description

This endpoint allows a user to follow another user. Once a follow relationship is established, the follower will be able to see the followed user's public content in their feed. Users cannot follow themselves.

## Authentication Requirements

- **Required**: Yes
- **Type**: Bearer Token
- **Header**: `Authorization: Bearer {token}`

## Request Parameters

### Path Parameters

None

### Query Parameters

| Parameter  | Type | Required | Description                               |
|------------|------|----------|-------------------------------------------|
| followerId | guid | Yes      | The ID of the user who wants to follow    |
| followedId | guid | Yes      | The ID of the user to be followed         |

### Request Body

None

## Response Format

### Success Response (201 Created)

```json
{
  "followerId": "guid",
  "followedId": "guid",
  "followedAt": "string (ISO datetime)",
  "follower": {
    "userId": "guid",
    "username": "string",
    "fullName": "string",
    "email": "string",
    "intro": "string",
    "image": "string (URL)",
    "joinedAt": "string (ISO datetime)"
  },
  "followed": {
    "userId": "guid",
    "username": "string",
    "fullName": "string",
    "email": "string",
    "intro": "string",
    "image": "string (URL)",
    "joinedAt": "string (ISO datetime)"
  }
}
```

### Field Descriptions

#### Follow Object

| Field      | Type      | Description                                         |
|------------|-----------|-----------------------------------------------------|
| followerId | guid      | ID of the user who is following                     |
| followedId | guid      | ID of the user being followed                       |
| followedAt | string    | Date and time when the follow occurred (ISO format) |
| follower   | UserDto   | Information about the follower                      |
| followed   | UserDto   | Information about the followed user                 |

#### User Object

| Field     | Type     | Description                                      |
|-----------|----------|--------------------------------------------------|
| userId    | guid     | Unique identifier for the user                   |
| username  | string   | Username of the user                             |
| fullName  | string   | Full name of the user                            |
| email     | string   | Email address of the user                        |
| intro     | string   | Brief user bio or introduction                   |
| image     | string   | URL to the user's profile image                  |
| joinedAt  | string   | Date and time when the user joined (ISO format)  |

### Error Responses

#### Bad Request (400)

```json
{
  "error": "Không thể theo dõi chính mình"
}
```

or 

```json
{
  "error": "Đã theo dõi người dùng này"
}
```

#### Unauthorized (401)

```json
{
  "error": "Invalid token."
}
```

#### Not Found (404)

```json
{
  "error": "Không tìm thấy người dùng"
}
```

#### Server Error (500)

```json
{
  "error": "An error occurred while processing the follow request."
}
```

## Examples

### Example 1: Follow a user

#### Request

```http
POST /api/Follows?followerId=4b516857-e734-40fe-887a-b3e9e3b1392a&followedId=9b2c5cda-e8c3-4361-9cde-72b3f0f84e44
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
{
  "followerId": "4b516857-e734-40fe-887a-b3e9e3b1392a",
  "followedId": "9b2c5cda-e8c3-4361-9cde-72b3f0f84e44",
  "followedAt": "2025-05-23T08:15:30Z",
  "follower": {
    "userId": "4b516857-e734-40fe-887a-b3e9e3b1392a",
    "username": "johndoe",
    "fullName": "John Doe",
    "email": "john.doe@example.com",
    "intro": "Photography enthusiast and avid traveler",
    "image": "https://socialmediastoragebinh.blob.core.windows.net/user-uploads/1747887520780-boy.jpg",
    "joinedAt": "2025-04-10T14:30:20Z"
  },
  "followed": {
    "userId": "9b2c5cda-e8c3-4361-9cde-72b3f0f84e44",
    "username": "janedoe",
    "fullName": "Jane Doe",
    "email": "jane.doe@example.com",
    "intro": "Digital artist and tech blogger",
    "image": "https://socialmediastoragebinh.blob.core.windows.net/user-uploads/1747887789683-girl.jpg",
    "joinedAt": "2025-03-15T09:45:12Z"
  }
}
```

### Example 2: Attempting to follow yourself

#### Request

```http
POST /api/Follows?followerId=4b516857-e734-40fe-887a-b3e9e3b1392a&followedId=4b516857-e734-40fe-887a-b3e9e3b1392a
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
{
  "error": "Không thể theo dõi chính mình"
}
```

### Example 3: Attempting to follow a user you already follow

#### Request

```http
POST /api/Follows?followerId=4b516857-e734-40fe-887a-b3e9e3b1392a&followedId=9b2c5cda-e8c3-4361-9cde-72b3f0f84e44
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
{
  "error": "Đã theo dõi người dùng này"
}
```

## Implementation Notes

- The endpoint requires authentication with a valid JWT token.
- Users cannot follow themselves - an error will be returned if `followerId` equals `followedId`.
- If a user tries to follow someone they already follow, an error will be returned.
- A successful follow operation returns the complete follow object with details about both users.
- The `followedAt` timestamp is set to the current UTC time when the follow operation is performed.
- Follow relationships are directional - if User A follows User B, it doesn't automatically mean User B follows User A.
- The endpoint returns a 201 Created status code on success, indicating that a new follow relationship has been created.

## Best Practices

### Frontend Implementation

1. **User Interface**:
   - Implement clear follow/unfollow buttons on user profiles
   - Update button state immediately after a successful follow action
   - Display follower counts to show popularity
   - Consider adding a notification system for new followers

2. **Error Handling**:
   - Display appropriate error messages when the follow action fails
   - Handle cases where users try to follow themselves or users they already follow
   - Provide clear feedback about the state of the follow action

3. **User Experience**:
   - Consider implementing a "follow back" suggestion when someone follows a user
   - Show the following status clearly on user profiles
   - Update UI immediately on follow/unfollow to provide instant feedback

### Integration Notes

- Use this endpoint in conjunction with the unfollow endpoint (`DELETE /api/Follows`) to manage follow relationships.
- After following a user, update the user's feed to include content from the followed user.
- The follow relationship affects which users' content appears in a user's feed.
- Consider implementing notifications when a user is followed by someone.
- For analytics, track follow/unfollow patterns to understand user relationships and engagement.
