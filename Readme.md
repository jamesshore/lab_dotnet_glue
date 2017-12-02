The Lab: .NET Glue Code
===========

This repository contains an example of a simple back-end glue layer in C# and .NET. The code is for a back-end service that takes two lists of emails in JSON, collates and de-dupes them, then uses an Auth0 service to reset their passwords. It demonstrates several concepts:

1. **Focused Integration Tests.** The project contains a REST client, `RestClient.cs`. Its tests launch a small web server and verify that HTTP calls are performed correctly.

2. **Mock-Based Unit Tests for Infrastructure.** The Auth0 client is tested by mocking out `RestClient`.

3. **Mock-Free Unit Tests for Logic.** All business logic is in `EmailCollator.cs`, and it is tested without using mocks.

4. **Mock-Light Design to Minimize the Need for Mocks.** Business logic has been separated from infrastructure so mocks are kept to a minimum. To be more specific, a typical implementation's dependency graph would look like this:

	```
	1. Entry point --> 2. Collation Logic --> 3. Auth0 Client --> 4. Rest Client
	```

	But this project lifts the collation logic to the top level, freeing it from having any infrastructure dependencies. This allows the collation logic to be tested without using mocks:

	```
	                  2. Collation Logic
	                 /
	1. Entry point  +
	                 \
	                  3. Auth0 Client --> 4. Rest Client
  ```

  In a larger, more complicated application, additional logic would be attached to the `2` logic branch above, allowing it all to be tested without the use of mocks, and limiting the portion of the code that needed mock-based testing to the entry point and `3` infrastructure branch only.

Because of the destructive nature of this code--it actually resets Auth0 passwords--I have only tested the code, not tried it end-to-end. However, it is based on a real-world Node.JS project that ran successfully in 2016.


Setup
-----

To try this code on your own computer:

1. Install Visual Studio.
2. Download and unzip the source code into a convenient directory.
3. Run Visual Studio as admin (Important! The tests launch a web server, which can't be done without admin permissions.)
4. Run the tests (Ctrl-R, A)


Finding Your Way Around
-----------------------

The `Auth0Glue` project contains the production code and `Auth0Glue.Tests` contains the test code. There's a 1:1 relationship between test classes and production classes. There are four classes:

* `MassPasswordReset` The application entry point.
* `EmailCollator` Collation logic.
* `Auth0Client` Data Access Object (DAO) for the Auth0 service. Just implements the password reset endpoint.
* `RestClient` Minimalistic REST client.


License
-------

MIT License. See `LICENSE.TXT`.

