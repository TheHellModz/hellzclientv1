using System;
using System.Collections.Generic;
using System.Text;
using PlayFab;
using PlayFab.Public;
using PlayFab.ClientModels;
using UnityEngine;
using Steamworks;
using PlayFab.Internal;
using PlayFab.MultiplayerModels;
using PlayFab.SharedModels;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.AuthenticationModels;
using ExitGames.Client.Photon.StructWrapping;

namespace Hellz_Client.Mods
{
    internal class AntiBan
    {

        public static event PlayFabHttp.ApiProcessingEvent<ApiProcessingEventArgs> ApiProcessingEventHandler;
        public static List<CallRequestContainer> _apiCallQueue = new List<CallRequestContainer>();
        private PlayFabAuthenticationContext? authenticationContext;
        private PlayFabApiSettings? apiSettings;

        protected internal static void SendEvent(string apiEndpoint, PlayFabRequestCommon request, PlayFabResultCommon result, ApiProcessingEventType eventType)
        {
            if (ApiProcessingEventHandler == null)
            {
                return;
            }
            try
            {
                ApiProcessingEventHandler(new ApiProcessingEventArgs
                {
                    ApiEndpoint = apiEndpoint,
                    EventType = eventType,
                    Request = request,
                    Result = result
                });
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }


        public static void _MakeApiCall<TResult>(string apiEndpoint, string fullUrl, PlayFabRequestCommon request, AuthType authType, Action<TResult> resultCallback, Action<PlayFabError> errorCallback, object customData, Dictionary<string, string> extraHeaders, bool allowQueueing, PlayFabAuthenticationContext authenticationContext, PlayFabApiSettings apiSettings, IPlayFabInstanceApi instanceApi) where TResult : PlayFabResultCommon
        {
            PlayFabHttp.InitializeHttp();
            SendEvent(apiEndpoint, request, null, ApiProcessingEventType.Pre);
            ISerializerPlugin serializer = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer, "");
            CallRequestContainer reqContainer = new CallRequestContainer
            {
                ApiEndpoint = apiEndpoint,
                FullUrl = fullUrl,
                settings = apiSettings,
                context = authenticationContext,
                CustomData = customData,
                Payload = Encoding.UTF8.GetBytes(serializer.SerializeObject(request)),
                ApiRequest = request,
                ErrorCallback = errorCallback,
                RequestHeaders = (extraHeaders ?? new Dictionary<string, string>()),
                instanceApi = instanceApi
            };
            foreach (KeyValuePair<string, string> keyValuePair in PlayFabHttp.GlobalHeaderInjection)
            {
                if (!reqContainer.RequestHeaders.ContainsKey(keyValuePair.Key))
                {
                    reqContainer.RequestHeaders[keyValuePair.Key] = keyValuePair.Value;
                }
            }
            ITransportPlugin plugin = PluginManager.GetPlugin<ITransportPlugin>(PluginContract.PlayFab_Transport, "");
            reqContainer.RequestHeaders["X-ReportErrorAsSuccess"] = "true";
            reqContainer.RequestHeaders["X-PlayFabSDK"] = "UnitySDK-2.87.200602";
            if (authType != AuthType.LoginSession)
            {
                if (authType == AuthType.EntityToken)
                {
                    if (authenticationContext != null)
                    {
                        reqContainer.RequestHeaders["X-EntityToken"] = authenticationContext.EntityToken;
                    }
                }
            }
            else if (authenticationContext != null)
            {
                reqContainer.RequestHeaders["X-Authorization"] = authenticationContext.ClientSessionTicket;
            }
            reqContainer.DeserializeResultJson = delegate ()
            {
                reqContainer.ApiResult = serializer.DeserializeObject<TResult>(reqContainer.JsonResponse);
            };
            reqContainer.InvokeSuccessCallback = delegate ()
            {
                if (resultCallback != null)
                {
                    resultCallback((TResult)((object)reqContainer.ApiResult));
                }
            };
            if (allowQueueing && _apiCallQueue != null)
            {
                for (int i = _apiCallQueue.Count - 1; i >= 0; i--)
                {
                    if (_apiCallQueue[i].ApiEndpoint == apiEndpoint)
                    {
                        _apiCallQueue.RemoveAt(i);
                    }
                }
                _apiCallQueue.Add(reqContainer);
                return;
            }
            plugin.MakeApiCall(reqContainer);
        }

        protected internal static void MakeApiCall<TResult>(string apiEndpoint, PlayFabRequestCommon request, AuthType authType, Action<TResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null, PlayFabAuthenticationContext authenticationContext = null, PlayFabApiSettings apiSettings = null, IPlayFabInstanceApi instanceApi = null) where TResult : PlayFabResultCommon
        {
            apiSettings = (apiSettings ?? PlayFabSettings.staticSettings);
            string fullUrl = apiSettings.GetFullUrl(apiEndpoint, apiSettings.RequestGetParams);
            _MakeApiCall<TResult>(apiEndpoint, fullUrl, request, authType, resultCallback, errorCallback, customData, extraHeaders, false, authenticationContext, apiSettings, instanceApi);
        }

        public static void getEntityToken()
        {
            GetEntityTokenRequest request = new GetEntityTokenRequest();
            request.Entity.Id = null;
        }

        public void ShutdownMultiplayerServer(ShutdownMultiplayerServerRequest request, Action<PlayFab.ClientModels.EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
            PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
            if (!playFabAuthenticationContext.IsEntityLoggedIn())
            {
                throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
            }
            MakeApiCall<PlayFab.ClientModels.EmptyResponse>("/MultiplayerServer/ShutdownMultiplayerServer", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings);
        }


        public static void CurrentRoom(Photon.Realtime.Room crp)
        {

        }


        public enum PlayFabErrorCode
        {
            // Token: 0x0400017C RID: 380
            Unknown = 1,
            // Token: 0x0400017D RID: 381
            ConnectionError,
            // Token: 0x0400017E RID: 382
            JsonParseError,
            // Token: 0x0400017F RID: 383
            Success = 0,
            // Token: 0x04000180 RID: 384
            UnkownError = 500,
            // Token: 0x04000181 RID: 385
            InvalidParams = 1000,
            // Token: 0x04000182 RID: 386
            AccountNotFound,
            // Token: 0x04000183 RID: 387
            AccountBanned, /// IMPORTANT
            // Token: 0x04000184 RID: 388
            InvalidUsernameOrPassword,
            // Token: 0x04000185 RID: 389
            InvalidTitleId,
            // Token: 0x04000186 RID: 390
            InvalidEmailAddress,
            // Token: 0x04000187 RID: 391
            EmailAddressNotAvailable,
            // Token: 0x04000188 RID: 392
            InvalidUsername,
            // Token: 0x04000189 RID: 393
            InvalidPassword,
            // Token: 0x0400018A RID: 394
            UsernameNotAvailable,
            // Token: 0x0400018B RID: 395
            InvalidSteamTicket,
            // Token: 0x0400018C RID: 396
            AccountAlreadyLinked,
            // Token: 0x0400018D RID: 397
            LinkedAccountAlreadyClaimed,
            // Token: 0x0400018E RID: 398
            InvalidFacebookToken,
            // Token: 0x0400018F RID: 399
            AccountNotLinked,
            // Token: 0x04000190 RID: 400
            FailedByPaymentProvider,
            // Token: 0x04000191 RID: 401
            CouponCodeNotFound,
            // Token: 0x04000192 RID: 402
            InvalidContainerItem,
            // Token: 0x04000193 RID: 403
            ContainerNotOwned,
            // Token: 0x04000194 RID: 404
            KeyNotOwned,
            // Token: 0x04000195 RID: 405
            InvalidItemIdInTable,
            // Token: 0x04000196 RID: 406
            InvalidReceipt,
            // Token: 0x04000197 RID: 407
            ReceiptAlreadyUsed,
            // Token: 0x04000198 RID: 408
            ReceiptCancelled,
            // Token: 0x04000199 RID: 409
            GameNotFound,
            // Token: 0x0400019A RID: 410
            GameModeNotFound,
            // Token: 0x0400019B RID: 411
            InvalidGoogleToken,
            // Token: 0x0400019C RID: 412
            UserIsNotPartOfDeveloper,
            // Token: 0x0400019D RID: 413
            InvalidTitleForDeveloper,
            // Token: 0x0400019E RID: 414
            TitleNameConflicts,
            // Token: 0x0400019F RID: 415
            UserisNotValid,
            // Token: 0x040001A0 RID: 416
            ValueAlreadyExists,
            // Token: 0x040001A1 RID: 417
            BuildNotFound,
            // Token: 0x040001A2 RID: 418
            PlayerNotInGame,
            // Token: 0x040001A3 RID: 419
            InvalidTicket,
            // Token: 0x040001A4 RID: 420
            InvalidDeveloper,
            // Token: 0x040001A5 RID: 421
            InvalidOrderInfo,
            // Token: 0x040001A6 RID: 422
            RegistrationIncomplete,
            // Token: 0x040001A7 RID: 423
            InvalidPlatform,
            // Token: 0x040001A8 RID: 424
            UnknownError,
            // Token: 0x040001A9 RID: 425
            SteamApplicationNotOwned,
            // Token: 0x040001AA RID: 426
            WrongSteamAccount,
            // Token: 0x040001AB RID: 427
            TitleNotActivated,
            // Token: 0x040001AC RID: 428
            RegistrationSessionNotFound,
            // Token: 0x040001AD RID: 429
            NoSuchMod,
            // Token: 0x040001AE RID: 430
            FileNotFound,
            // Token: 0x040001AF RID: 431
            DuplicateEmail,
            // Token: 0x040001B0 RID: 432
            ItemNotFound,
            // Token: 0x040001B1 RID: 433
            ItemNotOwned,
            // Token: 0x040001B2 RID: 434
            ItemNotRecycleable,
            // Token: 0x040001B3 RID: 435
            ItemNotAffordable,
            // Token: 0x040001B4 RID: 436
            InvalidVirtualCurrency,
            // Token: 0x040001B5 RID: 437
            WrongVirtualCurrency,
            // Token: 0x040001B6 RID: 438
            WrongPrice,
            // Token: 0x040001B7 RID: 439
            NonPositiveValue,
            // Token: 0x040001B8 RID: 440
            InvalidRegion,
            // Token: 0x040001B9 RID: 441
            RegionAtCapacity,
            // Token: 0x040001BA RID: 442
            ServerFailedToStart,
            // Token: 0x040001BB RID: 443
            NameNotAvailable,
            // Token: 0x040001BC RID: 444
            InsufficientFunds,
            // Token: 0x040001BD RID: 445
            InvalidDeviceID,
            // Token: 0x040001BE RID: 446
            InvalidPushNotificationToken,
            // Token: 0x040001BF RID: 447
            NoRemainingUses,
            // Token: 0x040001C0 RID: 448
            InvalidPaymentProvider,
            // Token: 0x040001C1 RID: 449
            PurchaseInitializationFailure,
            // Token: 0x040001C2 RID: 450
            DuplicateUsername,
            // Token: 0x040001C3 RID: 451
            InvalidBuyerInfo,
            // Token: 0x040001C4 RID: 452
            NoGameModeParamsSet,
            // Token: 0x040001C5 RID: 453
            BodyTooLarge,
            // Token: 0x040001C6 RID: 454
            ReservedWordInBody,
            // Token: 0x040001C7 RID: 455
            InvalidTypeInBody,
            // Token: 0x040001C8 RID: 456
            InvalidRequest,
            // Token: 0x040001C9 RID: 457
            ReservedEventName,
            // Token: 0x040001CA RID: 458
            InvalidUserStatistics,
            // Token: 0x040001CB RID: 459
            NotAuthenticated,
            // Token: 0x040001CC RID: 460
            StreamAlreadyExists,
            // Token: 0x040001CD RID: 461
            ErrorCreatingStream,
            // Token: 0x040001CE RID: 462
            StreamNotFound,
            // Token: 0x040001CF RID: 463
            InvalidAccount,
            // Token: 0x040001D0 RID: 464
            PurchaseDoesNotExist = 1080,
            // Token: 0x040001D1 RID: 465
            InvalidPurchaseTransactionStatus,
            // Token: 0x040001D2 RID: 466
            APINotEnabledForGameClientAccess,
            // Token: 0x040001D3 RID: 467
            NoPushNotificationARNForTitle,
            // Token: 0x040001D4 RID: 468
            BuildAlreadyExists,
            // Token: 0x040001D5 RID: 469
            BuildPackageDoesNotExist,
            // Token: 0x040001D6 RID: 470
            CustomAnalyticsEventsNotEnabledForTitle = 1087,
            // Token: 0x040001D7 RID: 471
            InvalidSharedGroupId,
            // Token: 0x040001D8 RID: 472
            NotAuthorized,
            // Token: 0x040001D9 RID: 473
            MissingTitleGoogleProperties,
            // Token: 0x040001DA RID: 474
            InvalidItemProperties,
            // Token: 0x040001DB RID: 475
            InvalidPSNAuthCode,
            // Token: 0x040001DC RID: 476
            InvalidItemId,
            // Token: 0x040001DD RID: 477
            PushNotEnabledForAccount,
            // Token: 0x040001DE RID: 478
            PushServiceError,
            // Token: 0x040001DF RID: 479
            ReceiptDoesNotContainInAppItems,
            // Token: 0x040001E0 RID: 480
            ReceiptContainsMultipleInAppItems,
            // Token: 0x040001E1 RID: 481
            InvalidBundleID,
            // Token: 0x040001E2 RID: 482
            JavascriptException,
            // Token: 0x040001E3 RID: 483
            InvalidSessionTicket,
            // Token: 0x040001E4 RID: 484
            UnableToConnectToDatabase,
            // Token: 0x040001E5 RID: 485
            InternalServerError = 1110,
            // Token: 0x040001E6 RID: 486
            InvalidReportDate,
            // Token: 0x040001E7 RID: 487
            ReportNotAvailable,
            // Token: 0x040001E8 RID: 488
            DatabaseThroughputExceeded,
            // Token: 0x040001E9 RID: 489
            InvalidGameTicket = 1115,
            // Token: 0x040001EA RID: 490
            ExpiredGameTicket,
            // Token: 0x040001EB RID: 491
            GameTicketDoesNotMatchLobby,
            // Token: 0x040001EC RID: 492
            LinkedDeviceAlreadyClaimed,
            // Token: 0x040001ED RID: 493
            DeviceAlreadyLinked,
            // Token: 0x040001EE RID: 494
            DeviceNotLinked,
            // Token: 0x040001EF RID: 495
            PartialFailure,
            // Token: 0x040001F0 RID: 496
            PublisherNotSet,
            // Token: 0x040001F1 RID: 497
            ServiceUnavailable,
            // Token: 0x040001F2 RID: 498
            VersionNotFound,
            // Token: 0x040001F3 RID: 499
            RevisionNotFound,
            // Token: 0x040001F4 RID: 500
            InvalidPublisherId,
            // Token: 0x040001F5 RID: 501
            DownstreamServiceUnavailable,
            // Token: 0x040001F6 RID: 502
            APINotIncludedInTitleUsageTier,
            // Token: 0x040001F7 RID: 503
            DAULimitExceeded,
            // Token: 0x040001F8 RID: 504
            APIRequestLimitExceeded,
            // Token: 0x040001F9 RID: 505
            InvalidAPIEndpoint,
            // Token: 0x040001FA RID: 506
            BuildNotAvailable,
            // Token: 0x040001FB RID: 507
            ConcurrentEditError,
            // Token: 0x040001FC RID: 508
            ContentNotFound,
            // Token: 0x040001FD RID: 509
            CharacterNotFound,
            // Token: 0x040001FE RID: 510
            CloudScriptNotFound,
            // Token: 0x040001FF RID: 511
            ContentQuotaExceeded,
            // Token: 0x04000200 RID: 512
            InvalidCharacterStatistics,
            // Token: 0x04000201 RID: 513
            PhotonNotEnabledForTitle,
            // Token: 0x04000202 RID: 514
            PhotonApplicationNotFound,
            // Token: 0x04000203 RID: 515
            PhotonApplicationNotAssociatedWithTitle,
            // Token: 0x04000204 RID: 516
            InvalidEmailOrPassword,
            // Token: 0x04000205 RID: 517
            FacebookAPIError,
            // Token: 0x04000206 RID: 518
            InvalidContentType,
            // Token: 0x04000207 RID: 519
            KeyLengthExceeded,
            // Token: 0x04000208 RID: 520
            DataLengthExceeded,
            // Token: 0x04000209 RID: 521
            TooManyKeys,
            // Token: 0x0400020A RID: 522
            FreeTierCannotHaveVirtualCurrency,
            // Token: 0x0400020B RID: 523
            MissingAmazonSharedKey,
            // Token: 0x0400020C RID: 524
            AmazonValidationError,
            // Token: 0x0400020D RID: 525
            InvalidPSNIssuerId,
            // Token: 0x0400020E RID: 526
            PSNInaccessible,
            // Token: 0x0400020F RID: 527
            ExpiredAuthToken,
            // Token: 0x04000210 RID: 528
            FailedToGetEntitlements,
            // Token: 0x04000211 RID: 529
            FailedToConsumeEntitlement,
            // Token: 0x04000212 RID: 530
            TradeAcceptingUserNotAllowed,
            // Token: 0x04000213 RID: 531
            TradeInventoryItemIsAssignedToCharacter,
            // Token: 0x04000214 RID: 532
            TradeInventoryItemIsBundle,
            // Token: 0x04000215 RID: 533
            TradeStatusNotValidForCancelling,
            // Token: 0x04000216 RID: 534
            TradeStatusNotValidForAccepting,
            // Token: 0x04000217 RID: 535
            TradeDoesNotExist,
            // Token: 0x04000218 RID: 536
            TradeCancelled,
            // Token: 0x04000219 RID: 537
            TradeAlreadyFilled,
            // Token: 0x0400021A RID: 538
            TradeWaitForStatusTimeout,
            // Token: 0x0400021B RID: 539
            TradeInventoryItemExpired,
            // Token: 0x0400021C RID: 540
            TradeMissingOfferedAndAcceptedItems,
            // Token: 0x0400021D RID: 541
            TradeAcceptedItemIsBundle,
            // Token: 0x0400021E RID: 542
            TradeAcceptedItemIsStackable,
            // Token: 0x0400021F RID: 543
            TradeInventoryItemInvalidStatus,
            // Token: 0x04000220 RID: 544
            TradeAcceptedCatalogItemInvalid,
            // Token: 0x04000221 RID: 545
            TradeAllowedUsersInvalid,
            // Token: 0x04000222 RID: 546
            TradeInventoryItemDoesNotExist,
            // Token: 0x04000223 RID: 547
            TradeInventoryItemIsConsumed,
            // Token: 0x04000224 RID: 548
            TradeInventoryItemIsStackable,
            // Token: 0x04000225 RID: 549
            TradeAcceptedItemsMismatch,
            // Token: 0x04000226 RID: 550
            InvalidKongregateToken,
            // Token: 0x04000227 RID: 551
            FeatureNotConfiguredForTitle,
            // Token: 0x04000228 RID: 552
            NoMatchingCatalogItemForReceipt,
            // Token: 0x04000229 RID: 553
            InvalidCurrencyCode,
            // Token: 0x0400022A RID: 554
            NoRealMoneyPriceForCatalogItem,
            // Token: 0x0400022B RID: 555
            TradeInventoryItemIsNotTradable,
            // Token: 0x0400022C RID: 556
            TradeAcceptedCatalogItemIsNotTradable,
            // Token: 0x0400022D RID: 557
            UsersAlreadyFriends,
            // Token: 0x0400022E RID: 558
            LinkedIdentifierAlreadyClaimed,
            // Token: 0x0400022F RID: 559
            CustomIdNotLinked,
            // Token: 0x04000230 RID: 560
            TotalDataSizeExceeded,
            // Token: 0x04000231 RID: 561
            DeleteKeyConflict,
            // Token: 0x04000232 RID: 562
            InvalidXboxLiveToken,
            // Token: 0x04000233 RID: 563
            ExpiredXboxLiveToken,
            // Token: 0x04000234 RID: 564
            ResettableStatisticVersionRequired,
            // Token: 0x04000235 RID: 565
            NotAuthorizedByTitle,
            // Token: 0x04000236 RID: 566
            NoPartnerEnabled,
            // Token: 0x04000237 RID: 567
            InvalidPartnerResponse,
            // Token: 0x04000238 RID: 568
            APINotEnabledForGameServerAccess,
            // Token: 0x04000239 RID: 569
            StatisticNotFound,
            // Token: 0x0400023A RID: 570
            StatisticNameConflict,
            // Token: 0x0400023B RID: 571
            StatisticVersionClosedForWrites,
            // Token: 0x0400023C RID: 572
            StatisticVersionInvalid,
            // Token: 0x0400023D RID: 573
            APIClientRequestRateLimitExceeded,
            // Token: 0x0400023E RID: 574
            InvalidJSONContent,
            // Token: 0x0400023F RID: 575
            InvalidDropTable,
            // Token: 0x04000240 RID: 576
            StatisticVersionAlreadyIncrementedForScheduledInterval,
            // Token: 0x04000241 RID: 577
            StatisticCountLimitExceeded,
            // Token: 0x04000242 RID: 578
            StatisticVersionIncrementRateExceeded,
            // Token: 0x04000243 RID: 579
            ContainerKeyInvalid,
            // Token: 0x04000244 RID: 580
            CloudScriptExecutionTimeLimitExceeded,
            // Token: 0x04000245 RID: 581
            NoWritePermissionsForEvent,
            // Token: 0x04000246 RID: 582
            CloudScriptFunctionArgumentSizeExceeded,
            // Token: 0x04000247 RID: 583
            CloudScriptAPIRequestCountExceeded,
            // Token: 0x04000248 RID: 584
            CloudScriptAPIRequestError,
            // Token: 0x04000249 RID: 585
            CloudScriptHTTPRequestError,
            // Token: 0x0400024A RID: 586
            InsufficientGuildRole,
            // Token: 0x0400024B RID: 587
            GuildNotFound,
            // Token: 0x0400024C RID: 588
            OverLimit,
            // Token: 0x0400024D RID: 589
            EventNotFound,
            // Token: 0x0400024E RID: 590
            InvalidEventField,
            // Token: 0x0400024F RID: 591
            InvalidEventName,
            // Token: 0x04000250 RID: 592
            CatalogNotConfigured,
            // Token: 0x04000251 RID: 593
            OperationNotSupportedForPlatform,
            // Token: 0x04000252 RID: 594
            SegmentNotFound,
            // Token: 0x04000253 RID: 595
            StoreNotFound,
            // Token: 0x04000254 RID: 596
            InvalidStatisticName,
            // Token: 0x04000255 RID: 597
            TitleNotQualifiedForLimit,
            // Token: 0x04000256 RID: 598
            InvalidServiceLimitLevel,
            // Token: 0x04000257 RID: 599
            ServiceLimitLevelInTransition,
            // Token: 0x04000258 RID: 600
            CouponAlreadyRedeemed,
            // Token: 0x04000259 RID: 601
            GameServerBuildSizeLimitExceeded,
            // Token: 0x0400025A RID: 602
            GameServerBuildCountLimitExceeded,
            // Token: 0x0400025B RID: 603
            VirtualCurrencyCountLimitExceeded,
            // Token: 0x0400025C RID: 604
            VirtualCurrencyCodeExists,
            // Token: 0x0400025D RID: 605
            TitleNewsItemCountLimitExceeded,
            // Token: 0x0400025E RID: 606
            InvalidTwitchToken,
            // Token: 0x0400025F RID: 607
            TwitchResponseError,
            // Token: 0x04000260 RID: 608
            ProfaneDisplayName,
            // Token: 0x04000261 RID: 609
            UserAlreadyAdded,
            // Token: 0x04000262 RID: 610
            InvalidVirtualCurrencyCode,
            // Token: 0x04000263 RID: 611
            VirtualCurrencyCannotBeDeleted,
            // Token: 0x04000264 RID: 612
            IdentifierAlreadyClaimed,
            // Token: 0x04000265 RID: 613
            IdentifierNotLinked,
            // Token: 0x04000266 RID: 614
            InvalidContinuationToken,
            // Token: 0x04000267 RID: 615
            ExpiredContinuationToken,
            // Token: 0x04000268 RID: 616
            InvalidSegment,
            // Token: 0x04000269 RID: 617
            InvalidSessionId,
            // Token: 0x0400026A RID: 618
            SessionLogNotFound,
            // Token: 0x0400026B RID: 619
            InvalidSearchTerm,
            // Token: 0x0400026C RID: 620
            TwoFactorAuthenticationTokenRequired,
            // Token: 0x0400026D RID: 621
            GameServerHostCountLimitExceeded,
            // Token: 0x0400026E RID: 622
            PlayerTagCountLimitExceeded,
            // Token: 0x0400026F RID: 623
            RequestAlreadyRunning,
            // Token: 0x04000270 RID: 624
            ActionGroupNotFound,
            // Token: 0x04000271 RID: 625
            MaximumSegmentBulkActionJobsRunning,
            // Token: 0x04000272 RID: 626
            NoActionsOnPlayersInSegmentJob,
            // Token: 0x04000273 RID: 627
            DuplicateStatisticName,
            // Token: 0x04000274 RID: 628
            ScheduledTaskNameConflict,
            // Token: 0x04000275 RID: 629
            ScheduledTaskCreateConflict,
            // Token: 0x04000276 RID: 630
            InvalidScheduledTaskName,
            // Token: 0x04000277 RID: 631
            InvalidTaskSchedule,
            // Token: 0x04000278 RID: 632
            SteamNotEnabledForTitle,
            // Token: 0x04000279 RID: 633
            LimitNotAnUpgradeOption,
            // Token: 0x0400027A RID: 634
            NoSecretKeyEnabledForCloudScript,
            // Token: 0x0400027B RID: 635
            TaskNotFound,
            // Token: 0x0400027C RID: 636
            TaskInstanceNotFound,
            // Token: 0x0400027D RID: 637
            InvalidIdentityProviderId,
            // Token: 0x0400027E RID: 638
            MisconfiguredIdentityProvider,
            // Token: 0x0400027F RID: 639
            InvalidScheduledTaskType,
            // Token: 0x04000280 RID: 640
            BillingInformationRequired,
            // Token: 0x04000281 RID: 641
            LimitedEditionItemUnavailable,
            // Token: 0x04000282 RID: 642
            InvalidAdPlacementAndReward,
            // Token: 0x04000283 RID: 643
            AllAdPlacementViewsAlreadyConsumed,
            // Token: 0x04000284 RID: 644
            GoogleOAuthNotConfiguredForTitle,
            // Token: 0x04000285 RID: 645
            GoogleOAuthError,
            // Token: 0x04000286 RID: 646
            UserNotFriend,
            // Token: 0x04000287 RID: 647
            InvalidSignature,
            // Token: 0x04000288 RID: 648
            InvalidPublicKey,
            // Token: 0x04000289 RID: 649
            GoogleOAuthNoIdTokenIncludedInResponse,
            // Token: 0x0400028A RID: 650
            StatisticUpdateInProgress,
            // Token: 0x0400028B RID: 651
            LeaderboardVersionNotAvailable,
            // Token: 0x0400028C RID: 652
            StatisticAlreadyHasPrizeTable = 1279,
            // Token: 0x0400028D RID: 653
            PrizeTableHasOverlappingRanks,
            // Token: 0x0400028E RID: 654
            PrizeTableHasMissingRanks,
            // Token: 0x0400028F RID: 655
            PrizeTableRankStartsAtZero,
            // Token: 0x04000290 RID: 656
            InvalidStatistic,
            // Token: 0x04000291 RID: 657
            ExpressionParseFailure,
            // Token: 0x04000292 RID: 658
            ExpressionInvokeFailure,
            // Token: 0x04000293 RID: 659
            ExpressionTooLong,
            // Token: 0x04000294 RID: 660
            DataUpdateRateExceeded,
            // Token: 0x04000295 RID: 661
            RestrictedEmailDomain,
            // Token: 0x04000296 RID: 662
            EncryptionKeyDisabled,
            // Token: 0x04000297 RID: 663
            EncryptionKeyMissing,
            // Token: 0x04000298 RID: 664
            EncryptionKeyBroken,
            // Token: 0x04000299 RID: 665
            NoSharedSecretKeyConfigured,
            // Token: 0x0400029A RID: 666
            SecretKeyNotFound,
            // Token: 0x0400029B RID: 667
            PlayerSecretAlreadyConfigured,
            // Token: 0x0400029C RID: 668
            APIRequestsDisabledForTitle,
            // Token: 0x0400029D RID: 669
            InvalidSharedSecretKey,
            // Token: 0x0400029E RID: 670
            PrizeTableHasNoRanks,
            // Token: 0x0400029F RID: 671
            ProfileDoesNotExist,
            // Token: 0x040002A0 RID: 672
            ContentS3OriginBucketNotConfigured,
            // Token: 0x040002A1 RID: 673
            InvalidEnvironmentForReceipt,
            // Token: 0x040002A2 RID: 674
            EncryptedRequestNotAllowed,
            // Token: 0x040002A3 RID: 675
            SignedRequestNotAllowed,
            // Token: 0x040002A4 RID: 676
            RequestViewConstraintParamsNotAllowed,
            // Token: 0x040002A5 RID: 677
            BadPartnerConfiguration,
            // Token: 0x040002A6 RID: 678
            XboxBPCertificateFailure,
            // Token: 0x040002A7 RID: 679
            XboxXASSExchangeFailure,
            // Token: 0x040002A8 RID: 680
            InvalidEntityId,
            // Token: 0x040002A9 RID: 681
            StatisticValueAggregationOverflow,
            // Token: 0x040002AA RID: 682
            EmailMessageFromAddressIsMissing,
            // Token: 0x040002AB RID: 683
            EmailMessageToAddressIsMissing,
            // Token: 0x040002AC RID: 684
            SmtpServerAuthenticationError,
            // Token: 0x040002AD RID: 685
            SmtpServerLimitExceeded,
            // Token: 0x040002AE RID: 686
            SmtpServerInsufficientStorage,
            // Token: 0x040002AF RID: 687
            SmtpServerCommunicationError,
            // Token: 0x040002B0 RID: 688
            SmtpServerGeneralFailure,
            // Token: 0x040002B1 RID: 689
            EmailClientTimeout,
            // Token: 0x040002B2 RID: 690
            EmailClientCanceledTask,
            // Token: 0x040002B3 RID: 691
            EmailTemplateMissing,
            // Token: 0x040002B4 RID: 692
            InvalidHostForTitleId,
            // Token: 0x040002B5 RID: 693
            EmailConfirmationTokenDoesNotExist,
            // Token: 0x040002B6 RID: 694
            EmailConfirmationTokenExpired,
            // Token: 0x040002B7 RID: 695
            AccountDeleted,
            // Token: 0x040002B8 RID: 696
            PlayerSecretNotConfigured,
            // Token: 0x040002B9 RID: 697
            InvalidSignatureTime,
            // Token: 0x040002BA RID: 698
            NoContactEmailAddressFound,
            // Token: 0x040002BB RID: 699
            InvalidAuthToken,
            // Token: 0x040002BC RID: 700
            AuthTokenDoesNotExist,
            // Token: 0x040002BD RID: 701
            AuthTokenExpired,
            // Token: 0x040002BE RID: 702
            AuthTokenAlreadyUsedToResetPassword,
            // Token: 0x040002BF RID: 703
            MembershipNameTooLong,
            // Token: 0x040002C0 RID: 704
            MembershipNotFound,
            // Token: 0x040002C1 RID: 705
            GoogleServiceAccountInvalid,
            // Token: 0x040002C2 RID: 706
            GoogleServiceAccountParseFailure,
            // Token: 0x040002C3 RID: 707
            EntityTokenMissing,
            // Token: 0x040002C4 RID: 708
            EntityTokenInvalid,
            // Token: 0x040002C5 RID: 709
            EntityTokenExpired,
            // Token: 0x040002C6 RID: 710
            EntityTokenRevoked,
            // Token: 0x040002C7 RID: 711
            InvalidProductForSubscription,
            // Token: 0x040002C8 RID: 712
            XboxInaccessible,
            // Token: 0x040002C9 RID: 713
            SubscriptionAlreadyTaken,
            // Token: 0x040002CA RID: 714
            SmtpAddonNotEnabled,
            // Token: 0x040002CB RID: 715
            APIConcurrentRequestLimitExceeded,
            // Token: 0x040002CC RID: 716
            XboxRejectedXSTSExchangeRequest,
            // Token: 0x040002CD RID: 717
            VariableNotDefined,
            // Token: 0x040002CE RID: 718
            TemplateVersionNotDefined,
            // Token: 0x040002CF RID: 719
            FileTooLarge,
            // Token: 0x040002D0 RID: 720
            TitleDeleted,
            // Token: 0x040002D1 RID: 721
            TitleContainsUserAccounts,
            // Token: 0x040002D2 RID: 722
            TitleDeletionPlayerCleanupFailure,
            // Token: 0x040002D3 RID: 723
            EntityFileOperationPending,
            // Token: 0x040002D4 RID: 724
            NoEntityFileOperationPending,
            // Token: 0x040002D5 RID: 725
            EntityProfileVersionMismatch,
            // Token: 0x040002D6 RID: 726
            TemplateVersionTooOld,
            // Token: 0x040002D7 RID: 727
            MembershipDefinitionInUse,
            // Token: 0x040002D8 RID: 728
            PaymentPageNotConfigured,
            // Token: 0x040002D9 RID: 729
            FailedLoginAttemptRateLimitExceeded,
            // Token: 0x040002DA RID: 730
            EntityBlockedByGroup,
            // Token: 0x040002DB RID: 731
            RoleDoesNotExist,
            // Token: 0x040002DC RID: 732
            EntityIsAlreadyMember,
            // Token: 0x040002DD RID: 733
            DuplicateRoleId,
            // Token: 0x040002DE RID: 734
            GroupInvitationNotFound,
            // Token: 0x040002DF RID: 735
            GroupApplicationNotFound,
            // Token: 0x040002E0 RID: 736
            OutstandingInvitationAcceptedInstead,
            // Token: 0x040002E1 RID: 737
            OutstandingApplicationAcceptedInstead,
            // Token: 0x040002E2 RID: 738
            RoleIsGroupDefaultMember,
            // Token: 0x040002E3 RID: 739
            RoleIsGroupAdmin,
            // Token: 0x040002E4 RID: 740
            RoleNameNotAvailable,
            // Token: 0x040002E5 RID: 741
            GroupNameNotAvailable,
            // Token: 0x040002E6 RID: 742
            EmailReportAlreadySent,
            // Token: 0x040002E7 RID: 743
            EmailReportRecipientBlacklisted,
            // Token: 0x040002E8 RID: 744
            EventNamespaceNotAllowed,
            // Token: 0x040002E9 RID: 745
            EventEntityNotAllowed,
            // Token: 0x040002EA RID: 746
            InvalidEntityType,
            // Token: 0x040002EB RID: 747
            NullTokenResultFromAad,
            // Token: 0x040002EC RID: 748
            InvalidTokenResultFromAad,
            // Token: 0x040002ED RID: 749
            NoValidCertificateForAad,
            // Token: 0x040002EE RID: 750
            InvalidCertificateForAad,
            // Token: 0x040002EF RID: 751
            DuplicateDropTableId,
            // Token: 0x040002F0 RID: 752
            MultiplayerServerError,
            // Token: 0x040002F1 RID: 753
            MultiplayerServerTooManyRequests,
            // Token: 0x040002F2 RID: 754
            MultiplayerServerNoContent,
            // Token: 0x040002F3 RID: 755
            MultiplayerServerBadRequest,
            // Token: 0x040002F4 RID: 756
            MultiplayerServerUnauthorized,
            // Token: 0x040002F5 RID: 757
            MultiplayerServerForbidden,
            // Token: 0x040002F6 RID: 758
            MultiplayerServerNotFound,
            // Token: 0x040002F7 RID: 759
            MultiplayerServerConflict,
            // Token: 0x040002F8 RID: 760
            MultiplayerServerInternalServerError,
            // Token: 0x040002F9 RID: 761
            MultiplayerServerUnavailable,
            // Token: 0x040002FA RID: 762
            ExplicitContentDetected,
            // Token: 0x040002FB RID: 763
            PIIContentDetected,
            // Token: 0x040002FC RID: 764
            InvalidScheduledTaskParameter,
            // Token: 0x040002FD RID: 765
            PerEntityEventRateLimitExceeded,
            // Token: 0x040002FE RID: 766
            TitleDefaultLanguageNotSet,
            // Token: 0x040002FF RID: 767
            EmailTemplateMissingDefaultVersion,
            // Token: 0x04000300 RID: 768
            FacebookInstantGamesIdNotLinked,
            // Token: 0x04000301 RID: 769
            InvalidFacebookInstantGamesSignature,
            // Token: 0x04000302 RID: 770
            FacebookInstantGamesAuthNotConfiguredForTitle,
            // Token: 0x04000303 RID: 771
            EntityProfileConstraintValidationFailed,
            // Token: 0x04000304 RID: 772
            TelemetryIngestionKeyPending,
            // Token: 0x04000305 RID: 773
            TelemetryIngestionKeyNotFound,
            // Token: 0x04000306 RID: 774
            StatisticChildNameInvalid = 1402,
            // Token: 0x04000307 RID: 775
            DataIntegrityError,
            // Token: 0x04000308 RID: 776
            VirtualCurrencyCannotBeSetToOlderVersion,
            // Token: 0x04000309 RID: 777
            VirtualCurrencyMustBeWithinIntegerRange,
            // Token: 0x0400030A RID: 778
            EmailTemplateInvalidSyntax,
            // Token: 0x0400030B RID: 779
            EmailTemplateMissingCallback,
            // Token: 0x0400030C RID: 780
            PushNotificationTemplateInvalidPayload,
            // Token: 0x0400030D RID: 781
            InvalidLocalizedPushNotificationLanguage,
            // Token: 0x0400030E RID: 782
            MissingLocalizedPushNotificationMessage,
            // Token: 0x0400030F RID: 783
            PushNotificationTemplateMissingPlatformPayload,
            // Token: 0x04000310 RID: 784
            PushNotificationTemplatePayloadContainsInvalidJson,
            // Token: 0x04000311 RID: 785
            PushNotificationTemplateContainsInvalidIosPayload,
            // Token: 0x04000312 RID: 786
            PushNotificationTemplateContainsInvalidAndroidPayload,
            // Token: 0x04000313 RID: 787
            PushNotificationTemplateIosPayloadMissingNotificationBody,
            // Token: 0x04000314 RID: 788
            PushNotificationTemplateAndroidPayloadMissingNotificationBody,
            // Token: 0x04000315 RID: 789
            PushNotificationTemplateNotFound,
            // Token: 0x04000316 RID: 790
            PushNotificationTemplateMissingDefaultVersion,
            // Token: 0x04000317 RID: 791
            PushNotificationTemplateInvalidSyntax,
            // Token: 0x04000318 RID: 792
            PushNotificationTemplateNoCustomPayloadForV1,
            // Token: 0x04000319 RID: 793
            NoLeaderboardForStatistic,
            // Token: 0x0400031A RID: 794
            TitleNewsMissingDefaultLanguage,
            // Token: 0x0400031B RID: 795
            TitleNewsNotFound,
            // Token: 0x0400031C RID: 796
            TitleNewsDuplicateLanguage,
            // Token: 0x0400031D RID: 797
            TitleNewsMissingTitleOrBody,
            // Token: 0x0400031E RID: 798
            TitleNewsInvalidLanguage,
            // Token: 0x0400031F RID: 799
            EmailRecipientBlacklisted,
            // Token: 0x04000320 RID: 800
            InvalidGameCenterAuthRequest,
            // Token: 0x04000321 RID: 801
            GameCenterAuthenticationFailed,
            // Token: 0x04000322 RID: 802
            CannotEnablePartiesForTitle,
            // Token: 0x04000323 RID: 803
            PartyError,
            // Token: 0x04000324 RID: 804
            PartyRequests,
            // Token: 0x04000325 RID: 805
            PartyNoContent,
            // Token: 0x04000326 RID: 806
            PartyBadRequest,
            // Token: 0x04000327 RID: 807
            PartyUnauthorized,
            // Token: 0x04000328 RID: 808
            PartyForbidden,
            // Token: 0x04000329 RID: 809
            PartyNotFound,
            // Token: 0x0400032A RID: 810
            PartyConflict,
            // Token: 0x0400032B RID: 811
            PartyInternalServerError,
            // Token: 0x0400032C RID: 812
            PartyUnavailable,
            // Token: 0x0400032D RID: 813
            PartyTooManyRequests,
            // Token: 0x0400032E RID: 814
            PushNotificationTemplateMissingName,
            // Token: 0x0400032F RID: 815
            CannotEnableMultiplayerServersForTitle,
            // Token: 0x04000330 RID: 816
            WriteAttemptedDuringExport,
            // Token: 0x04000331 RID: 817
            MultiplayerServerTitleQuotaCoresExceeded,
            // Token: 0x04000332 RID: 818
            AutomationRuleNotFound,
            // Token: 0x04000333 RID: 819
            EntityAPIKeyLimitExceeded,
            // Token: 0x04000334 RID: 820
            EntityAPIKeyNotFound,
            // Token: 0x04000335 RID: 821
            EntityAPIKeyOrSecretInvalid,
            // Token: 0x04000336 RID: 822
            EconomyServiceUnavailable,
            // Token: 0x04000337 RID: 823
            EconomyServiceInternalError,
            // Token: 0x04000338 RID: 824
            QueryRateLimitExceeded,
            // Token: 0x04000339 RID: 825
            EntityAPIKeyCreationDisabledForEntity,
            // Token: 0x0400033A RID: 826
            ForbiddenByEntityPolicy,
            // Token: 0x0400033B RID: 827
            UpdateInventoryRateLimitExceeded,
            // Token: 0x0400033C RID: 828
            StudioCreationRateLimited,
            // Token: 0x0400033D RID: 829
            StudioCreationInProgress,
            // Token: 0x0400033E RID: 830
            DuplicateStudioName,
            // Token: 0x0400033F RID: 831
            StudioNotFound,
            // Token: 0x04000340 RID: 832
            StudioDeleted,
            // Token: 0x04000341 RID: 833
            StudioDeactivated,
            // Token: 0x04000342 RID: 834
            StudioActivated,
            // Token: 0x04000343 RID: 835
            TitleCreationRateLimited,
            // Token: 0x04000344 RID: 836
            TitleCreationInProgress,
            // Token: 0x04000345 RID: 837
            DuplicateTitleName,
            // Token: 0x04000346 RID: 838
            TitleActivationRateLimited,
            // Token: 0x04000347 RID: 839
            TitleActivationInProgress,
            // Token: 0x04000348 RID: 840
            TitleDeactivated,
            // Token: 0x04000349 RID: 841
            TitleActivated,
            // Token: 0x0400034A RID: 842
            CloudScriptAzureFunctionsExecutionTimeLimitExceeded,
            // Token: 0x0400034B RID: 843
            CloudScriptAzureFunctionsArgumentSizeExceeded,
            // Token: 0x0400034C RID: 844
            CloudScriptAzureFunctionsReturnSizeExceeded,
            // Token: 0x0400034D RID: 845
            CloudScriptAzureFunctionsHTTPRequestError,
            // Token: 0x0400034E RID: 846
            VirtualCurrencyBetaGetError,
            // Token: 0x0400034F RID: 847
            VirtualCurrencyBetaCreateError,
            // Token: 0x04000350 RID: 848
            VirtualCurrencyBetaInitialDepositSaveError,
            // Token: 0x04000351 RID: 849
            VirtualCurrencyBetaSaveError,
            // Token: 0x04000352 RID: 850
            VirtualCurrencyBetaDeleteError,
            // Token: 0x04000353 RID: 851
            VirtualCurrencyBetaRestoreError,
            // Token: 0x04000354 RID: 852
            VirtualCurrencyBetaSaveConflict,
            // Token: 0x04000355 RID: 853
            VirtualCurrencyBetaUpdateError,
            // Token: 0x04000356 RID: 854
            InsightsManagementDatabaseNotFound,
            // Token: 0x04000357 RID: 855
            InsightsManagementOperationNotFound,
            // Token: 0x04000358 RID: 856
            InsightsManagementErrorPendingOperationExists,
            // Token: 0x04000359 RID: 857
            InsightsManagementSetPerformanceLevelInvalidParameter,
            // Token: 0x0400035A RID: 858
            InsightsManagementSetStorageRetentionInvalidParameter,
            // Token: 0x0400035B RID: 859
            InsightsManagementGetStorageUsageInvalidParameter,
            // Token: 0x0400035C RID: 860
            InsightsManagementGetOperationStatusInvalidParameter,
            // Token: 0x0400035D RID: 861
            DuplicatePurchaseTransactionId,
            // Token: 0x0400035E RID: 862
            EvaluationModePlayerCountExceeded,
            // Token: 0x0400035F RID: 863
            GetPlayersInSegmentRateLimitExceeded,
            // Token: 0x04000360 RID: 864
            CloudScriptFunctionNameSizeExceeded,
            // Token: 0x04000361 RID: 865
            InsightsManagementTitleInEvaluationMode,
            // Token: 0x04000362 RID: 866
            CloudScriptAzureFunctionsQueueRequestError,
            // Token: 0x04000363 RID: 867
            EvaluationModeTitleCountExceeded,
            // Token: 0x04000364 RID: 868
            InsightsManagementTitleNotInFlight,
            // Token: 0x04000365 RID: 869
            LimitNotFound,
            // Token: 0x04000366 RID: 870
            LimitNotAvailableViaAPI,
            // Token: 0x04000367 RID: 871
            InsightsManagementSetStorageRetentionBelowMinimum,
            // Token: 0x04000368 RID: 872
            InsightsManagementSetStorageRetentionAboveMaximum,
            // Token: 0x04000369 RID: 873
            AppleNotEnabledForTitle,
            // Token: 0x0400036A RID: 874
            InsightsManagementNewActiveEventExportLimitInvalid,
            // Token: 0x0400036B RID: 875
            InsightsManagementSetPerformanceRateLimited,
            // Token: 0x0400036C RID: 876
            PartyRequestsThrottledFromRateLimiter,
            // Token: 0x0400036D RID: 877
            XboxServiceTooManyRequests,
            // Token: 0x0400036E RID: 878
            NintendoSwitchNotEnabledForTitle,
            // Token: 0x0400036F RID: 879
            RequestMultiplayerServersThrottledFromRateLimiter,
            // Token: 0x04000370 RID: 880
            TitleDataInstanceNotFound,
            // Token: 0x04000371 RID: 881
            DuplicateTitleDataOverrideInstanceName,
            // Token: 0x04000372 RID: 882
            MatchmakingEntityInvalid = 2001,
            // Token: 0x04000373 RID: 883
            MatchmakingPlayerAttributesInvalid,
            // Token: 0x04000374 RID: 884
            MatchmakingQueueNotFound = 2016,
            // Token: 0x04000375 RID: 885
            MatchmakingMatchNotFound,
            // Token: 0x04000376 RID: 886
            MatchmakingTicketNotFound,
            // Token: 0x04000377 RID: 887
            MatchmakingAlreadyJoinedTicket = 2028,
            // Token: 0x04000378 RID: 888
            MatchmakingTicketAlreadyCompleted,
            // Token: 0x04000379 RID: 889
            MatchmakingQueueConfigInvalid = 2031,
            // Token: 0x0400037A RID: 890
            MatchmakingMemberProfileInvalid,
            // Token: 0x0400037B RID: 891
            NintendoSwitchDeviceIdNotLinked = 2034,
            // Token: 0x0400037C RID: 892
            MatchmakingNotEnabled,
            // Token: 0x0400037D RID: 893
            MatchmakingPlayerAttributesTooLarge = 2043,
            // Token: 0x0400037E RID: 894
            MatchmakingNumberOfPlayersInTicketTooLarge,
            // Token: 0x0400037F RID: 895
            MatchmakingAttributeInvalid = 2046,
            // Token: 0x04000380 RID: 896
            MatchmakingPlayerHasNotJoinedTicket = 2053,
            // Token: 0x04000381 RID: 897
            MatchmakingRateLimitExceeded,
            // Token: 0x04000382 RID: 898
            MatchmakingTicketMembershipLimitExceeded,
            // Token: 0x04000383 RID: 899
            MatchmakingUnauthorized,
            // Token: 0x04000384 RID: 900
            MatchmakingQueueLimitExceeded,
            // Token: 0x04000385 RID: 901
            MatchmakingRequestTypeMismatch,
            // Token: 0x04000386 RID: 902
            MatchmakingBadRequest,
            // Token: 0x04000387 RID: 903
            TitleConfigNotFound = 3001,
            // Token: 0x04000388 RID: 904
            TitleConfigUpdateConflict,
            // Token: 0x04000389 RID: 905
            TitleConfigSerializationError,
            // Token: 0x0400038A RID: 906
            CatalogEntityInvalid = 4001,
            // Token: 0x0400038B RID: 907
            CatalogTitleIdMissing,
            // Token: 0x0400038C RID: 908
            CatalogPlayerIdMissing,
            // Token: 0x0400038D RID: 909
            CatalogClientIdentityInvalid,
            // Token: 0x0400038E RID: 910
            CatalogOneOrMoreFilesInvalid,
            // Token: 0x0400038F RID: 911
            CatalogItemMetadataInvalid,
            // Token: 0x04000390 RID: 912
            CatalogItemIdInvalid,
            // Token: 0x04000391 RID: 913
            CatalogSearchParameterInvalid,
            // Token: 0x04000392 RID: 914
            CatalogFeatureDisabled,
            // Token: 0x04000393 RID: 915
            CatalogConfigInvalid,
            // Token: 0x04000394 RID: 916
            CatalogUnauthorized,
            // Token: 0x04000395 RID: 917
            CatalogItemTypeInvalid,
            // Token: 0x04000396 RID: 918
            CatalogBadRequest,
            // Token: 0x04000397 RID: 919
            CatalogTooManyRequests,
            // Token: 0x04000398 RID: 920
            ExportInvalidStatusUpdate = 5000,
            // Token: 0x04000399 RID: 921
            ExportInvalidPrefix,
            // Token: 0x0400039A RID: 922
            ExportBlobContainerDoesNotExist,
            // Token: 0x0400039B RID: 923
            ExportNotFound = 5004,
            // Token: 0x0400039C RID: 924
            ExportCouldNotUpdate,
            // Token: 0x0400039D RID: 925
            ExportInvalidStorageType,
            // Token: 0x0400039E RID: 926
            ExportAmazonBucketDoesNotExist,
            // Token: 0x0400039F RID: 927
            ExportInvalidBlobStorage,
            // Token: 0x040003A0 RID: 928
            ExportKustoException,
            // Token: 0x040003A1 RID: 929
            ExportKustoConnectionFailed = 5012,
            // Token: 0x040003A2 RID: 930
            ExportUnknownError,
            // Token: 0x040003A3 RID: 931
            ExportCantEditPendingExport,
            // Token: 0x040003A4 RID: 932
            ExportLimitExports,
            // Token: 0x040003A5 RID: 933
            ExportLimitEvents,
            // Token: 0x040003A6 RID: 934
            ExportInvalidPartitionStatusModification,
            // Token: 0x040003A7 RID: 935
            ExportCouldNotCreate,
            // Token: 0x040003A8 RID: 936
            ExportNoBackingDatabaseFound,
            // Token: 0x040003A9 RID: 937
            ExportCouldNotDelete,
            // Token: 0x040003AA RID: 938
            ExportCannotDetermineEventQuery,
            // Token: 0x040003AB RID: 939
            ExportInvalidQuerySchemaModification,
            // Token: 0x040003AC RID: 940
            ExportQuerySchemaMissingRequiredColumns,
            // Token: 0x040003AD RID: 941
            ExportCannotParseQuery,
            // Token: 0x040003AE RID: 942
            ExportControlCommandsNotAllowed,
            // Token: 0x040003AF RID: 943
            ExportQueryMissingTableReference,
            // Token: 0x040003B0 RID: 944
            TitleNotEnabledForParty = 6000,
            // Token: 0x040003B1 RID: 945
            PartyVersionNotFound,
            // Token: 0x040003B2 RID: 946
            MultiplayerServerBuildReferencedByMatchmakingQueue,
            // Token: 0x040003B3 RID: 947
            ExperimentationExperimentStopped = 7000,
            // Token: 0x040003B4 RID: 948
            ExperimentationExperimentRunning,
            // Token: 0x040003B5 RID: 949
            ExperimentationExperimentNotFound,
            // Token: 0x040003B6 RID: 950
            ExperimentationExperimentNeverStarted,
            // Token: 0x040003B7 RID: 951
            ExperimentationExperimentDeleted,
            // Token: 0x040003B8 RID: 952
            ExperimentationClientTimeout,
            // Token: 0x040003B9 RID: 953
            ExperimentationInvalidVariantConfiguration,
            // Token: 0x040003BA RID: 954
            ExperimentationInvalidVariableConfiguration,
            // Token: 0x040003BB RID: 955
            ExperimentInvalidId,
            // Token: 0x040003BC RID: 956
            ExperimentationNoScorecard,
            // Token: 0x040003BD RID: 957
            ExperimentationTreatmentAssignmentFailed,
            // Token: 0x040003BE RID: 958
            ExperimentationTreatmentAssignmentDisabled,
            // Token: 0x040003BF RID: 959
            ExperimentationInvalidDuration,
            // Token: 0x040003C0 RID: 960
            ExperimentationMaxExperimentsReached,
            // Token: 0x040003C1 RID: 961
            ExperimentationExperimentSchedulingInProgress,
            // Token: 0x040003C2 RID: 962
            ExperimentationExistingCodelessScheduled,
            // Token: 0x040003C3 RID: 963
            MaxActionDepthExceeded = 8000,
            // Token: 0x040003C4 RID: 964
            TitleNotOnUpdatedPricingPlan = 9000,
            // Token: 0x040003C5 RID: 965
            SnapshotNotFound = 11000
        }

    }
}
