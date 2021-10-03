// using System;
// using ADEUtility;
// using UnityEngine;
// using UnityEngine.EventSystems;
//
// public class TurretSpawner : MonoSingleton<TurretSpawner>
// {
// 	public event Action OnTurretsChanged;
//
// 	private const int MaxTurretAmount = 7;
// 	private TurretData current;
// 	private Ghost ghost;
// 	private int currentTurretAmount;
//
// 	private void Update()
// 	{
// 		if (!ghost)
// 			return;
//
// 		ghost.transform.position = Helper.GetMouseWorldPosition();
//
// 		if (Input.GetMouseButtonDown(0))
// 			TrySpawn();
// 	}
//
// 	public void SelectTurret(TurretData turretData)
// 	{
// 		current = null;
// 		current = turretData;
// 		Destroy(ghost != null ? ghost.gameObject : null);
//
// 		ghost = Instantiate(
// 			turretData.GhostPrefab,
// 			Helper.GetMouseWorldPosition(),
// 			turretData.GhostPrefab.transform.rotation
// 		);
//
// 		ghost.Display(current);
// 	}
//
// 	public void Spawn(TurretData turretData, Transform target)
// 	{
// 		Instantiate(turretData.Prefab, target.position, target.rotation);
// 	}
//
// 	public void DestroyTurret(Turret turret)
// 	{
// 		Destroy(turret.gameObject);
// 		turret.enabled = false;
// 		OnTurretsChanged?.Invoke();
// 	}
//
// 	private void TrySpawn()
// 	{
// 		if (EventSystem.current.IsPointerOverGameObject())
// 			return;
//
// 		if (currentTurretAmount >= MaxTurretAmount)
// 			return;
//
// 		Instantiate(current.Prefab, Helper.GetMouseWorldPosition(), ghost.transform.rotation);
// 		OnTurretsChanged?.Invoke();
// 		currentTurretAmount++;
// 		Destroy(ghost.gameObject);
// 		current = null;
// 	}
// }
