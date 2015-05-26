using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PurchaseTest : MonoBehaviour
{
	public Text btnText = null;

	public void Awake()
	{
		_Debug.mode = OutputMode.RUNTIME;
	}

	public void Start()
	{
		btnText.text = "INIT";

		AndroidInAppPurchaseManager.ActionBillingSetupFinished += _onBillingSetupFinished;
		AndroidInAppPurchaseManager.instance.addProduct("org.bestlogic.purchasez.item01");
		AndroidInAppPurchaseManager.instance.loadStore("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxn8ouJVeC1CwfwDcK3pL0roIShjBpO5giVYVlqqfIoL4MXHpqT5AmvKDNsUPNnn1RNOLcZc4MieBf8K0TmLUqENh6roX4rCJColyrgfNpYjFSqx52r1sghcDahlWVl03tlBK8OQkcJOZcnM3l5fb8WyZ0s27AIBt4Oj3G7aTfTkT/8KtT7MhYlzttmPCuNnygxf1BhHV2lZFPxDarKuut0fFRRmWATZgtruvZ3WVYUWmhgXaVvrPFovFK1qXMbNSasCiYYhetNqsSZ/BwetHqJBRi2mAtoZYvx88xBTyAZtOc3AMrBYrm3EHDUeptflGZs/KM1aPPj93sXyap6dr7QIDAQAB");
	}

	public void Purchase()
	{
		AndroidInAppPurchaseManager.instance.purchase("org.bestlogic.purchasez.item01");
	}

	private void _onBillingSetupFinished(BillingResult p_result)
	{
		AndroidInAppPurchaseManager.ActionBillingSetupFinished -= _onBillingSetupFinished;
		if (p_result.isSuccess)
		{
			AndroidInAppPurchaseManager.instance.retrieveProducDetails();
			AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += _onRetrieveProductsFinished;
		}
	}

	private void _onRetrieveProductsFinished(BillingResult p_result)
	{
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= _onRetrieveProductsFinished;
		if (p_result.isSuccess)
		{
			AndroidInAppPurchaseManager.ActionProductPurchased += _onProductPurchased;
			AndroidInAppPurchaseManager.ActionProductConsumed += _onProductConsumed;

			btnText.text = "BUY";
		}
	}

	private void _onProductPurchased(BillingResult p_result)
	{
		if (p_result.isSuccess || p_result.response == BillingResponseCodes.BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED)
		{
			GooglePurchaseTemplate l_purchase = p_result.purchase;
			AndroidInAppPurchaseManager.instance.consume(l_purchase.SKU);
		}
	}

	private void _onProductConsumed(BillingResult p_result)
	{
		if (p_result.isSuccess)
		{
			GooglePurchaseTemplate l_purchase = p_result.purchase;
			_Debug.log("---------------Product Info -----------------------");
			_Debug.log("package: " + l_purchase.packageName);
			_Debug.log("orderId: " + l_purchase.orderId);
			_Debug.log("token: " + l_purchase.token);
			_Debug.log("signature: " + l_purchase.signature);
			_Debug.log("payload: " + l_purchase.developerPayload);
			_Debug.log("json: " + l_purchase.originalJson);
			_Debug.log("---------------------------------------------------");
			
			// Send request to server to validate the receipt and pay the product to the user
			RequestQueue l_queue = new RequestQueue();
			l_queue.add(new PurchaseGemsRequest(l_purchase.packageName, l_purchase.orderId, l_purchase.SKU, l_purchase.token, l_purchase.developerPayload, _onPurchaseRequestComplete));
			l_queue.request(RequestType.RUSH);
		}
	}

	private void _onPurchaseRequestComplete(WWW p_response)
	{
		_Debug.log("Purchase Complete: " + p_response.text);
	}
}
