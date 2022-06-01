using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField]
    private int NumberOfInstantiatedProjectiles = 50;
    [SerializeField]
    private ProjectileComponent _projectile;

    private Queue<ProjectileComponent> ProjectileQueuePool = new Queue<ProjectileComponent>();

    private void Awake()
    {
        for (int i = 0; i < NumberOfInstantiatedProjectiles; i++)
        {
            var projectile = Instantiate(_projectile, transform);
            ProjectileQueuePool.Enqueue(projectile);
        }
    }

    public List<ProjectileComponent> GetProjectiles(int numsOfNeededProjectiles = 1)
    {
        List<ProjectileComponent> projectiles = new List<ProjectileComponent>();
        for (int i = 0; i < numsOfNeededProjectiles; i++)
        {
            ProjectileComponent projectile = ProjectileQueuePool.Dequeue();
            ProjectileQueuePool.Enqueue(projectile);
            projectiles.Add(projectile);
        }
        if (projectiles.Count > 0) return projectiles;
        return null;
    }

    public ProjectileComponent GetNearestProjectileInEnemyRadius(EnemyComponent unit)
    {
        float nearestDistance = float.MaxValue;
        ProjectileComponent nearestProjectile = null;
        foreach (ProjectileComponent projectile in ProjectileQueuePool)
        {
            var distance = Vector3.Distance(new Vector3(projectile.gameObject.transform.position.x, unit.transform.position.y, projectile.gameObject.transform.position.z), unit.transform.position);
            if (distance <= unit.GetPlayerIdentificationRadius && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestProjectile = projectile;
            }
        }
        return nearestProjectile;
    }
}
