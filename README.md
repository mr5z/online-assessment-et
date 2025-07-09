# Task A
![image](https://github.com/user-attachments/assets/faa43473-5b36-42cc-b2fe-cf0c524ba4f0)

# Task B
![image](https://github.com/user-attachments/assets/426840b1-e917-4d69-aee9-b39a6894a766)

#### Describe what could go wrong in rendering and how you'd fix it.
Blazor components rely heavily on state management, especially in Blazor Server projects where rendering depends on consistent server-side state. I used Fluxor, a Redux-style library, to centralize and manage state effectively.

In Blazor Server, state must be configured properly to avoid issues like UI resets or inconsistent rendering. This can be addressed by enabling distributed caching for scalability, but for this project, it only uses in-memory with a scoped lifetime.

Behind a load balancer, sticky sessions are required to keep users on the same server. This should avoid session-related rendering issues and works well with distributed caching.

For cases where component state should persist after a page reload, Fluxor store can easily be extended to save and restore state.

# Task C
![image](https://github.com/user-attachments/assets/9839dde2-0f6b-4e5c-9d53-b489c3ba1de4)
