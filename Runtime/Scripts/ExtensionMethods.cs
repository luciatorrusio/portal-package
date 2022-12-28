using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public static class ExtensionMethods
    {
        public static bool IsInFrontOf(this Transform one, Transform other)
        {
            
            var toOther = one.position - other.position;
            var dot = Vector3.Dot(other.forward, toOther);
            return dot > 0;
        }
        public static bool IsInFrontOfWithError(this Transform one, Transform other, float error)
        {
            var toOther = one.position - other.position;
            return Vector3.Dot(other.forward, toOther) > error ;
        }
        public static GameObject GetMainCamera(this Transform o)
        {
            foreach (var child in o.GetComponentsInChildren<Transform>())
            {
                if (child.CompareTag("MainCamera")) 
                    return child.gameObject;
            }
            return null;
        
        }

        public static void SetDestinationWithPortals(this NavMeshAgent navMeshAgent, Vector3 destination)
        {
            List<InPortal> inPortals = PortalManager.AllInPortals;
            var goDirectly = false;
            float minDistance = Mathf.Infinity;
            InPortal portalToUse = null;
            
            
            var totalDistance = 0f;
            NavMeshPath path = new NavMeshPath();
            NavMeshPath minPath = new NavMeshPath();
            NavMeshPath customPath = new NavMeshPath();
            
            // checkeo distancia yendo directamente al destination
            navMeshAgent.CalculatePath(destination, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                for (int i = 0; i < path.corners.Length-1; i++)
                    totalDistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                minDistance = totalDistance;
                goDirectly = true;
            }

            // todo: simplificar funcion usando agregate y fijarse de dejar de sumar distancia si ya es mas grande que minDistance
            // checkeo distancia yendo por portal
            foreach (var inPortal in inPortals)
            {
                // 1. me fijo si puede llegar a la entrada del portal
                totalDistance = 0f;
                navMeshAgent.CalculatePath(inPortal.transform.position, path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    for (int i = 0; i < path.corners.Length - 1; i++)
                        totalDistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                        
                    // 2. me fijo si desde la salida puede llegar al destination
                    NavMesh.CalculatePath(inPortal.GetLinkedOutPortal().transform.position, destination,NavMesh.AllAreas, path);
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        for (int i = 0; i < path.corners.Length-1; i++)
                            totalDistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                        if (totalDistance < minDistance)
                        {
                            goDirectly = false;
                            minDistance = totalDistance;
                            portalToUse = inPortal;
                            minPath = path;
                        }
                    }
                    
                }
                
                
                
            }

            if (float.IsPositiveInfinity(minDistance))
            {
                navMeshAgent.ResetPath();
                return;
            }
            
            if (goDirectly)
            {
                navMeshAgent.destination = destination;
                return;
            }

            
            navMeshAgent.destination = portalToUse.transform.position;

        }
        
        
        
    }
}