# remoteauthapi

Simple example API for authentication.

This app provides a single API endpoint.


## Endpoint /checkpassword

Send a POST request to this endpoint with a payload like this:

```json
{
    "username": "bob",
    "password": "bobspassword"
}
```

The API will check the password file and return a result document like this:

```json
{
    "success": true,
    "message": "OK"
}
```

or

```json
{
    "success": false,
    "message": "credentials invalid"
}
```


## Users file

The JSON file contains the users and their passwords:

```json
{
    "alice": "mysecret",
    "bob": "bobspassword",
    "claire": "cl41r3is4l337h4x0r"
}
```

## Environment variables

| var        | default    | description                                         |
| ---------- | ---------- | --------------------------------------------------- |
| API_ADDR   | localhost  | Address to bind to.                                 |
| API_PORT   | 8888       | Port number for the server.                         |
| USERS_FILE | users.json | Path to the JSON file with the users and passwords. |
