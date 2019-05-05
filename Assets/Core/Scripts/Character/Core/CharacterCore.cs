using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCore : MonoBehaviour
{
    #region Public Properties

    public static CharacterCore Instance { get; private set; }

    #endregion Public Properties

    #region Private Methods

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #endregion Private Methods

    #region Public Methods

    public void Move()
    {
        transform.Translate(transform.up);
    }

    #endregion Public Methods
}