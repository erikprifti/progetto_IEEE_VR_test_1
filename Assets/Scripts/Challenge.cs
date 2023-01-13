using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using Unity.VisualScripting;

public class Challenge : NetworkBehaviour
{
    public Porta porta;
    public IdKeyPairs idKeyPairs;

    [SyncVar]
    public string message;
    [SyncVar]
    public int activePlayerId;
    [SyncVar]
    public int passivePlayerId;
    [SyncVar]
    public string messageEncryptedP;
    [SyncVar]
    public string messageEncryptedA;
    
    public string doorPassword;

    public GameObject ConfirmButton;
    public GameObject StartButton;
    public GameObject Display;
   
    SyncList<long> tempP = new SyncList<long>();

    SyncList<long> tempA = new SyncList<long>();

    private void Start()
    {
        idKeyPairs = GameObject.FindWithTag("IdKeyPairs").GetComponent<IdKeyPairs>();
        porta = GameObject.FindWithTag("Porta").GetComponent<Porta>();
        StartButton = GameObject.FindWithTag("StartButton");

        ConfirmButton = GameObject.FindWithTag("ConfirmButton");


    }



    //for encryption and decryption
    long[] mb; //message in long array
    long[] ma;
    long[] en; //encrypted message in long array
    //public void setChallenge(int activePlayer, int passivePlayer)
    //{

    //    activePlayerId = activePlayer;
    //    passivePlayerId = passivePlayer;
    //    message = generateMessage();
    //    porta.setPassword(message);


    //    PlayerManager player = findPlayerById(activePlayerId);
    //    player.GetComponent<PlayerManager>().setPassword(message);

    //    Debug.LogError("IN SETCHALLENGE: " +activePlayer + " ha selezionato, mentre " + passivePlayer + " � stato selezionato");


    //    encrypt();
       
    //}

 

    private string generateMessage()
    {
        doorPassword = "Kebab";
        return "Kebab";
    }


    //private void encrypt()
    //{
    //    mb = new long[message.Length];
    //    ma = new long[message.Length];

    //    tempP = new SyncList<long>(); //temp mi salva i numeri che poi mi servono per decript
    //    en = new long[message.Length];
    //    long pt, ct, k;
    //    long key = idKeyPairs.getEncode(passivePlayerId);
    //    int n = idKeyPairs.getModule(passivePlayerId);
    //    int i = 0;

    //    for(int j = 0; j<message.Length; j++)
    //    {
    //        mb[j] = (int)message[j];
    //    }

    //    while (i < message.Length)
    //    {
    //        pt = mb[i];
    //        pt = pt - 64;
    //        k = 1;
    //        for (long j = 0; j < key; j++)
    //        {
    //            k = k * pt;
    //            k = k % n;
    //        }
    //        tempP[i] = k;
    //        ct = k + 64;
    //        en[i] = ct;
    //        i++;
    //    }

    //    string encryptMex = string.Empty;
    //    for (i = 0; i < en.Length; i++)
    //    {
    //        encryptMex = encryptMex + (char)en[i];
    //    }
    //    messageEncryptedP = encryptMex;

    //}

    //public string decrypt(int key)
    //{
    //    long pt, ct, k;
    //    int n = idKeyPairs.getModule(passivePlayerId);
    //    int i = 0;
    //    //invece che usare en e ma devo prendere la stringa encryptmessage e usarla come array
    //    while (i < en.Length) 
    //    {

    //        ct = tempP[i];
    //        k = 1;
    //        for (long j = 0; j < key; j++)
    //        {
    //            k = k * ct;
    //            k = k % n;
    //        }
    //        pt = k + 64;
    //        ma[i] = pt;
        
    //        i++;
    //    }

    //    string decryptMex = null;

    //    for (i = 0; i < ma.Length; i++)
    //    {
    //        decryptMex = decryptMex + (char)ma[i];
    //    }

    //    return decryptMex;
    //}

    private PlayerManager findPlayerById(int activePlayerId)
    {
        //Debug.LogError("activePlayer is " + activePlayerId);
        GameObject[] listaPlayer = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < listaPlayer.Length; i++)
        {
            //Debug.LogError(listaPlayer[i].GetComponent<PlayerManager>().getId());
            if (listaPlayer[i].GetComponent<PlayerManager>().getId().Equals(activePlayerId))
            {
                //Debug.LogError("trovato l'active " +listaPlayer[i].GetComponent<PlayerManager>().getId());
                return listaPlayer[i].GetComponent<PlayerManager>();
            }
        }
        return null;
    }

    public void resetChallenge()
    {
        message = null;
        activePlayerId = 0;
        passivePlayerId = 0;
        messageEncryptedP = null;
        messageEncryptedA = null;
        doorPassword = null;
        tempA.Clear();
        tempP.Clear();
    }

    public string play(int key, int Id) //CHIMATA IN SERVER
    {
        Debug.LogError("IN PLAY");
        PlayerManager p = findPlayerById(Id);

        if (activePlayerId == 0 && passivePlayerId == 0 ) //play chiamato dall attivo dopo aver messo la sua chiave privata con la quale decriptare per prima il messaggio
        {
            activePlayerId = p.id;
            string mex = generateMessage();
            messageEncryptedA = encrypt(mex, key, idKeyPairs.getModule(p.id), 0);
            porta.setPassword(doorPassword);
            return doorPassword;
            //p.setPassword(doorPassword);
        }
        else if(activePlayerId != 0 && passivePlayerId != 0)
        {
            //momento di decriptazione
            return resolveChallenge(key, p.gameObject);

        }else 
            return null;
    }

    public void sendMessage(GameObject player) //player passivo a cui mandare mex
    {
        int pId = player.GetComponent<PlayerManager>().id;
        passivePlayerId = pId;
        int publicKey = idKeyPairs.getEncode(passivePlayerId);
        messageEncryptedP = encrypt(messageEncryptedA, publicKey, idKeyPairs.getModule(pId), 1);
        message = messageEncryptedP;
        
    }

    private string encrypt(string mex, int key, int n, int caso)
    {

        mb = new long[mex.Length];
        SyncList<long> temp;

        if (caso == 0)
        {
            
            temp = tempA;
        }
        else
        {
            
            temp = tempP;
        }

        en = new long[mex.Length];
        long pt, ct, k;
        int i = 0;

        for (int j = 0; j < mex.Length; j++)
        {
            mb[j] = (int)mex[j];
        }

        while (i < mex.Length)
        {
            pt = mb[i];
            pt = pt - 64;
            k = 1;
            for (long j = 0; j < key; j++)
            {
                k = k * pt;
                k = k % n;
            }
            temp.Insert(i, k);
           // temp[i] = k;
            ct = k + 64;
            en[i] = ct;
            i++;
        }

        string encryptMex = string.Empty;
        for (i = 0; i < en.Length; i++)
        {
            encryptMex = encryptMex + (char)en[i];
        }

        return encryptMex;
    }

    public string decrypt(string mex, int key, int n, int caso)
    {
        long pt, ct, k;
        int i = 0;
        ma = new long[mex.Length];
        SyncList<long> temp;

        if (caso == 0)
        {
            temp = tempA;
        }
        else
        {
            temp = tempP;
        }
        //invece che usare en e ma devo prendere la stringa encryptmessage e usarla come array
        while (i < mex.Length)
        {

            ct = temp[i];
            k = 1;
            for (long j = 0; j < key; j++)
            {
                k = k * ct;
                k = k % n;
            }
            pt = k + 64;
            ma[i] = pt;

            i++;
        }

        string decryptMex = null;

        for (i = 0; i < ma.Length; i++)
        {
            decryptMex = decryptMex + (char)ma[i];
        }

        return decryptMex;
    }
    public string resolveChallenge(int privateKey, GameObject player) //chiamata da command
    {
        if (!player.CompareTag("Player")) { return null; }
        PlayerManager playerManager = player.GetComponent<PlayerManager>();
        int playerId = playerManager.getId();
        if (!playerId.Equals(passivePlayerId)) { return null; }

        string messageDecryptedP = decrypt(message, privateKey, idKeyPairs.getModule(passivePlayerId), 1);
        string messageDecryptedA = decrypt(messageDecryptedP, idKeyPairs.getEncode(activePlayerId), idKeyPairs.getModule(activePlayerId), 0);
        PlayerManager p = findPlayerById(playerId);
     //   p.setPassword(messageDecryptedA); //questo da verificare

        // player.GetComponent<PlayerNet>().cmdChallengeFree(gameObject);
        gameObject.GetComponent<Teleport>().rpcChallengeFree();
        return messageDecryptedA;
    }

    [TargetRpc]
    public void rpcSendMessageTarget(NetworkConnection target, GameObject player)
    {

        sendMessage(player);
    }

    [ClientRpc]
    public void rpcChallengeBusy()
    {
        Display.GetComponentInParent<MeshRenderer>().material = Display.GetComponent<Display>().red;
        Display.GetComponent<Display>().setDisplayBusy();
        StartButton.GetComponent<BoxCollider>().enabled = false;

    }

    [TargetRpc]
    public void rpcTargetChallengeNextMove(NetworkConnection target)
    {
        Display.GetComponentInParent<MeshRenderer>().material = Display.GetComponent<Display>().red;
        Display.GetComponent<Display>().setDisplayNextMove();
        ConfirmButton.GetComponent<BoxCollider>().enabled = false;
    }

    [TargetRpc]
    public void rpcTargetChallengeConfirm(NetworkConnection target)
    {
        Display.GetComponentInParent<MeshRenderer>().material = Display.GetComponent<Display>().red;
        Display.GetComponent<Display>().setDisplayConfirm();
        ConfirmButton.GetComponent<BoxCollider>().enabled = true;
    }
}
