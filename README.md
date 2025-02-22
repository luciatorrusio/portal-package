# What is Portal-Package?

Portal-Package is a tool for game developers in Unity to add Portals to their games without complications. 

It supports multiple portals linked however the developer wants.

# How do I install Portal-Package?
<details>
<summary>Clone the project!</summary>

#### Unity URP 2021.3.9 or newer
In this option you will have available demo scenes

1. in the terminal pasete:
```
    git clone https://github.com/luciatorrusio/portal-package.git
```
2. Open Unity hub `Add` `->` `Add Project from disk` and select on the directory that you have downloaded the project
3. In Unity Assets head over to `AlsetRGames/Demo/Scenes` and go through all the different scenes

</details>

<details>
<summary>Install via Package Manager</summary>
With this option you will NOT have available Demo scenes
  
#### Unity URP 2021.3.9 or newer

1. Open Package Manager window (Window | Package Manager)
2. Click `+` button on the upper-left of a window, and select "Add package from git URL..."
3. Enter the following URL and click `Add` button

```
https://github.com/luciatorrusio/portal-package.git?path=Assets/AlsetRGames/Portal
```

</details>


# How do I use Portal-Package?

Currently this tool is developed for URP. 
Head over to the prefabs folder in the portal package. Here you will find PortalManager prefab and a portal prefab.
The PortalManager will have to be present anywhere in the Hierarchy for portals to work. 

The portal prefab you should simply drag it wherever you want in the scene.


