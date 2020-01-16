using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab; // 弾のプレハブ
    [SerializeField] Transform gunBarrelEnd;  // 銃口(弾の発射位置)

    [SerializeField] ParticleSystem gunParticle; // 発射時演出
    [SerializeField] AudioSource gunAudioSource; // 発射音の音源

    // Update is called once per frame
    void Update()
    {
        // 入力に応じて弾を発射する
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // プレハブを元に、シーン上に弾を生成
        Instantiate(bulletPrefab, gunBarrelEnd.position, gunBarrelEnd.rotation);

        // 発射時演出を再生
        gunParticle.Play();

        // 発射時の音を再生
        gunAudioSource.Play();
    }
}