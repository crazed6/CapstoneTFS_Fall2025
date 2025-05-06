using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShooter : MonoBehaviour
{
    public ParticleSystem Muzzle;
    public ParticleSystem Bullet;
    public float BulletSpeed = 100f;

    public float Distance;
    public ParticleSystem HitEffect;

    public Transform BulletStartPoint;
    private void OnEnable()
    {
        ShootBullet();
    }
    private void Update()
    {
    }
    public void ShootBullet()
    {
        StartCoroutine(ShootBulletCoroutine());
    }
    public IEnumerator ShootBulletCoroutine()
    {
        var time = Distance / BulletSpeed;
        Instantiate(Muzzle, BulletStartPoint.position, BulletStartPoint.rotation);
        var bullet = Instantiate(Bullet, BulletStartPoint.position, BulletStartPoint.rotation);
        bullet.transform.parent = transform;
        var timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            bullet.transform.position += bullet.transform.forward * BulletSpeed * Time.deltaTime;
            yield return null;
        }
        //destroy bullet
        Destroy(bullet.gameObject);
        //instantiate hit effect
        Instantiate(HitEffect, bullet.transform.position, bullet.transform.rotation);
    }

}
