# PORTAL
by Lucia Torrusio and Rodrigo Rearden


Only objects with Rigidbody component can go through portals.
## Public Functions
### Events

#### OnPortalEnter 

It is called when an object touches a portal and starts going through it.
IPortal interface has to be implemented.

#### OnPortalExit
It is called when an object goes through a portal

#### OnPortalTransitioning
It is called every frame an object is transitioning through a portal

Usage example:
<pre><code class='language-cs'>
public class Example : MonoBehaviour, IPortal
{

    public void OnPortalEnter(GameObject portal)
    {
        print("entered portal");
    }

    public void OnPortalTransitioning(Portal portal)
    {
    }

    public void OnPortalExit(Portal portal)
    {
    }
</code></pre>

### Portal Functions
