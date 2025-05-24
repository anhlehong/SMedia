# API Usage Documentation: Get Users Following

## Endpoint

```
GET /api/Follows/following/{userId}
```

## Description

This endpoint retrieves a paginated list of users that the specified user is following. It returns details about each follow relationship as well as information about the followed users.

## Authentication Requirements

- **Required**: Yes
- **Type**: Bearer Token
- **Header**: `Authorization: Bearer {token}`

## Request Parameters

### Path Parameters

| Parameter | Type | Required | Description                                         |
|-----------|------|----------|-----------------------------------------------------|
| userId    | guid | Yes      | The ID of the user whose follows you want to retrieve |

### Query Parameters

| Parameter | Type    | Required | Default | Description                                   |
|-----------|---------|----------|---------|-----------------------------------------------|
| skip      | integer | No       | 0       | The number of records to skip (for pagination) |
| take      | integer | No       | 10      | The number of records to return               |

### Request Body

None

## Response Format

### Success Response (200 OK)

```json
[
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
  },
  ...
]
```

### Field Descriptions

#### Follow Object

| Field      | Type      | Description                                         |
|------------|-----------|-----------------------------------------------------|
| followerId | guid      | ID of the user who is following (matches userId)    |
| followedId | guid      | ID of the user being followed                       |
| followedAt | string    | Date and time when the follow occurred (ISO format) |
| follower   | UserDto   | Information about the follower user                 |
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

#### Unauthorized (401)

```json
{
  "error": "Invalid token."
}
```

#### Not Found (404)

```json
{
  "error": "User not found."
}
```

#### Server Error (500)

```json
{
  "error": "An error occurred while retrieving following users."
}
```

## Examples

### Example 1: Get the first 10 users a user is following

#### Request

```http
GET /api/Follows/following/4b516857-e734-40fe-887a-b3e9e3b1392a
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
[
  {
    "followerId": "4b516857-e734-40fe-887a-b3e9e3b1392a",
    "followedId": "9b2c5cda-e8c3-4361-9cde-72b3f0f84e44",
    "followedAt": "2025-05-20T08:15:30Z",
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
  },
  {
    "followerId": "4b516857-e734-40fe-887a-b3e9e3b1392a",
    "followedId": "7d1c5cda-f8a2-4361-9cde-72b3f0f84e22",
    "followedAt": "2025-05-18T16:42:15Z",
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
      "userId": "7d1c5cda-f8a2-4361-9cde-72b3f0f84e22",
      "username": "bobsmith",
      "fullName": "Bob Smith",
      "email": "bob.smith@example.com",
      "intro": "Software developer and coffee aficionado",
      "image": "https://socialmediastoragebinh.blob.core.windows.net/user-uploads/1747887612345-profile.jpg",
      "joinedAt": "2025-01-05T11:20:33Z"
    }
  }
]
```

### Example 2: Get users with pagination

#### Request

```http
GET /api/Follows/following/4b516857-e734-40fe-887a-b3e9e3b1392a?skip=10&take=5
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
[
  {
    "followerId": "4b516857-e734-40fe-887a-b3e9e3b1392a",
    "followedId": "8a1c5cda-d7b2-4361-9cde-72b3f0f84d33",
    "followedAt": "2025-05-10T11:05:20Z",
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
      "userId": "8a1c5cda-d7b2-4361-9cde-72b3f0f84d33",
      "username": "alicejones",
      "fullName": "Alice Jones",
      "email": "alice.jones@example.com",
      "intro": "Nature photographer and hiker",
      "image": "https://socialmediastoragebinh.blob.core.windows.net/user-uploads/1747887654321-alice.jpg",
      "joinedAt": "2025-02-25T13:15:40Z"
    }
  },
  // Additional follow entries would appear here
]
```

### Example 3: User with no followed users

#### Request

```http
GET /api/Follows/following/5ea85f64-9913-4562-b3fc-3g963f66bdc8
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response

```json
[]
```

## Implementation Notes

- The endpoint requires authentication with a valid JWT token.
- The response is a paginated list of follow relationships, with each entry containing details about both the follower and followed users.
- The `skip` parameter allows skipping a number of records, useful for implementing pagination.
- The `take` parameter specifies how many records to return in a single request.
- If a user is not following anyone, an empty array will be returned rather than an error.
- Follow relationships are sorted by the `followedAt` date, with the most recent follows appearing first.
- Both the follower and followed user details are included in each record for convenience.

## Best Practices

### Frontend Implementation

1. **Pagination UI**:
   - Implement next/previous page buttons or infinite scrolling
   - Display a loading indicator during data fetching
   - Show the total number of followed users if available

2. **User Display**:
   - Show profile pictures and names of followed users
   - Include follow/unfollow toggles for each user
   - Group users by categories or interests if applicable

3. **Empty State Handling**:
   - Display a helpful message when a user is not following anyone
   - Provide suggestions for users to follow
   - Explain the benefits of following other users

### Integration Notes

- Use this endpoint in conjunction with the follow count endpoint (`GET /api/Follows/following/count/{userId}`) to display the total count of followed users.
- Consider implementing client-side caching for followed users to reduce API calls.
- This endpoint pairs well with the unfollow endpoint (`DELETE /api/Follows`) to manage follow relationships.
- When displaying a user's profile, you can use this endpoint to show a sample of users they are following.
- For a social feed, this endpoint helps determine which users' content should appear in a user's feed.
