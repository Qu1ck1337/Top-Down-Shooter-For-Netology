using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField]
    private int NumberOfInstantiatedProjectiles = 50;
    [SerializeField]
    private ProjectileComponent _projectile;

    private Queue<ProjectileComponent> ProjectileStackPool = new Queue<ProjectileComponent>();

    private void Start()
    {
        for (int i = 0; i < NumberOfInstantiatedProjectiles; i++)
        {
            var projectile = Instantiate(_projectile, transform);
            ProjectileStackPool.Enqueue(projectile);
        }
    }

    public List<ProjectileComponent> GetProjectiles(int numsOfNeededProjectiles = 1)
    {
        List<ProjectileComponent> projectiles = new List<ProjectileComponent>();
        for (int i = 0; i < numsOfNeededProjectiles; i++)
        {
            ProjectileComponent projectile = ProjectileStackPool.Dequeue();
            ProjectileStackPool.Enqueue(projectile);
            projectiles.Add(projectile);
        }
        if (projectiles.Count > 0) return projectiles;
        return null;
    }
}
