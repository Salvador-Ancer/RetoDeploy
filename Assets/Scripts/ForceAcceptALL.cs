using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic; 
using System.Collections;


public class ForceAcceptAll : CertificateHandler
{

    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
