using Project.Combat.Weapons;
using UnityEngine;

namespace Project.Ships
{
    public enum RelativeSide
    {
        Left,
        Right,
        Center
    }

    public class AttachPoint : MonoBehaviour
    {
        // public GameObject PreAttachedWeapon;
        public GameObject AttachedWeapon;
        [HideInInspector] public bool IsEmpty = true;

        public RelativeSide Side;

        private void Awake()
        {
            // Log the local position of the AttachPoint
            Vector3 localPosition = transform.localPosition;

            // Set the relative side based on the localPosition.x value
            if (localPosition.x < 0)
            {
                Side = RelativeSide.Left;
            }
            else if (localPosition.x > 0)
            {
                Side = RelativeSide.Right;
            }
            else
            {
                Side = RelativeSide.Center;
            }
        }

        public void InitialiseAttachPoint()
        {
            if (AttachedWeapon != null)
            {
                AttachedWeapon.GetComponent<WeaponBase>().Side = Side;
                IsEmpty = false;
            }
        }

        public void AttachWeapon(GameObject weaponPrefab, bool force)
        {
            if (!IsEmpty)
            {
                if (force == true)
                {
                    DetachWeapon();
                }
                else
                {
                    Debug.LogWarning("The weapon slot is already filled.");
                    return;
                }
            }

            GameObject weaponObject = Instantiate(weaponPrefab, transform.position, transform.rotation, transform);
            if (weaponObject != null)
            {
                WeaponBase weaponComponent = weaponObject.GetComponent<WeaponBase>();

                // Set the Side property of the weapon based on the AttachPoint
                weaponComponent.Side = Side;
                AttachedWeapon = weaponObject;
                IsEmpty = false;
            }
            else
            {
                Debug.LogError("The weaponPrefab does not have a WeaponBase component.");
                Destroy(weaponObject);
            }
        }

        public void DetachWeapon()
        {
            if (IsEmpty)
            {
                Debug.LogWarning("No weapon is attached to this slot.");
                return;
            }

            // Destroy the GameObject the WeaponBase script is attached to
            Destroy(AttachedWeapon.gameObject);
            AttachedWeapon = null;
            IsEmpty = true;
        }
    }
}