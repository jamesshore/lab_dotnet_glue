Notes to include in readme:

* Must run tests as admin
* TestHarnessServer implementation was complicated by threading issues. May not be best or most idiomatic way of solving the problem.
* Reduces need for mocks by putting not having infrastructure (Auth0Client) be a dependency of logic (EmailCollator)