using System;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;

    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Transform muzzleFlashSpawnPoint;

    [SerializeField] private Collider playerCollider;

    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlashPrefab;


    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;

    private BoxCollider projectileServerCollider;
    private BoxCollider projectileClientCollider;

    private bool shouldFire;
    private float previousFireTime;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        { return; }

        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        { return; }

        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    private void Update()
    {
        if (!IsOwner || !shouldFire)
        { return; }

        if (Time.time < 1/ fireRate + previousFireTime)
            { return; }

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.forward);
        SpawnProjectile(projectileSpawnPoint.position, projectileSpawnPoint.forward);
        previousFireTime = Time.time;
    }

    private void SpawnProjectile(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject muzzleFlashSpawned = Instantiate(muzzleFlashPrefab, muzzleFlashSpawnPoint);
        muzzleFlashSpawned.transform.parent = null;

        var projectileInstance = Instantiate(clientProjectilePrefab, spawnPosition, Quaternion.identity);

        direction = direction.normalized;

        // Set the projectile to look in the specified direction
        projectileInstance.transform.rotation = Quaternion.LookRotation(direction);

        if (projectileInstance.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.linearVelocity = rigidbody.transform.forward * projectileSpeed;
        }

        // Set the projectile to look in the specified direction
        projectileInstance.transform.rotation = Quaternion.LookRotation(direction);
        
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPosition, Vector3 direction)
    {
        var projectileInstance = Instantiate(serverProjectilePrefab, spawnPosition, Quaternion.identity);
        
        direction = direction.normalized;        
        // Set the projectile to look in the specified direction
        projectileInstance.transform.rotation = Quaternion.LookRotation(direction);

        if(projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }

        if (projectileInstance.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.linearVelocity = rigidbody.transform.forward * projectileSpeed;
        }
        PrimaryFireClientRpc(spawnPosition, direction);
    }

    [ClientRpc]
    private void PrimaryFireClientRpc(Vector3 spawnPosition, Vector3 direction)
    {
        if (IsOwner) { return; }
        var projectileInstance = Instantiate(clientProjectilePrefab, spawnPosition, Quaternion.identity);
        direction = direction.normalized;

        // Set the projectile to look in the specified direction
        projectileInstance.transform.rotation = Quaternion.LookRotation(direction);
        
        if(projectileInstance.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.linearVelocity = rigidbody.transform.forward * projectileSpeed;
        }
    }
    private void IgnoreProjectileCollisions(GameObject projectile1, GameObject projectile2)
    {
        Collider[] colliders1 = projectile1.GetComponentsInChildren<Collider>();
        Collider[] colliders2 = projectile2.GetComponentsInChildren<Collider>();

        foreach (var collider1 in colliders1)
        {
            foreach (var collider2 in colliders2)
            {
                Physics.IgnoreCollision(collider1, collider2);
            }
         }
    }
}