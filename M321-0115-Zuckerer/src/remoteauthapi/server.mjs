import { createServer } from "node:http";
import { readFileSync } from "node:fs";

const main = () => {
  const port = process.env.API_PORT ?? 8888;
  const host = process.env.API_ADDR ?? "localhost";
  const usersfile = process.env.USERS_FILE ?? "users.json";

  const userDb = JSON.parse(readFileSync(usersfile));

  const server = createServer();
  server.on("request", makeRequestHandler(userDb));
  server.listen(port, host);

  console.log(
    `Server is running at \x1b[32m%s\x1b[0m`,
    `http://${host}:${port}`
  );
};

const makeRequestHandler = (userDb) => async (request, response) => {
  if (request.url !== "/checkpassword") {
    response.writeHead(404);
    response.end("404 - Not found");
    return;
  }
  if (request.method !== "POST") {
    response.writeHead(405);
    response.end("405 - Method not allowed");
    return;
  }

  try {
    const data = await readBodyOfRequest(request);
    const { username, password } = JSON.parse(data);
    console.log(`${new Date().toISOString()} checking password for: ${username}`)
    const success = password === userDb[username];
    const result = { success, message: success ? "OK" : "credentials invalid" };
    response.writeHead(200, { "Content-Type": "application/json" });
    response.end(JSON.stringify(result));
    return;
  } catch (ex) {
    console.error(`exception: ${ex}`);
    response.writeHead(400);
    response.end("400 - Bad request");
    return;
  }
};

const readBodyOfRequest = (request) =>
  new Promise((resolve, reject) => {
    let body = "";
    request.on("data", (chunk) => (body += chunk));
    request.on("end", () => {
      resolve(body);
    });
    request.on("error", (err) => reject(err));
  });

// start application
main();
