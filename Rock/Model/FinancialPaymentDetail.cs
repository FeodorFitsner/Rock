﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Rock.Data;
using Rock.Financial;
using Rock.Security;
using Rock.Web.Cache;

namespace Rock.Model
{
    /// <summary>
    /// Represents details about the bank account or credit card that was used to make a payment
    /// </summary>
    [RockDomain( "Finance" )]
    [Table( "FinancialPaymentDetail" )]
    [DataContract]
    public partial class FinancialPaymentDetail : Model<FinancialPaymentDetail>
    {
        #region Entity Properties

        /// <summary>
        /// Gets or sets the Masked Account Number (Last 4 of Account Number prefixed with 12 *'s)
        /// </summary>
        /// <value>
        /// The account number masked.
        /// </value>
        [DataMember]
        public string AccountNumberMasked { get; set; }

        /// <summary>
        /// Gets or sets the DefinedValueId of the currency type <see cref="Rock.Model.DefinedValue"/> indicating the currency that the
        /// transaction was made in.
        /// </summary>
        /// <value>
        /// A <see cref="System.Int32" /> representing the DefinedValueId of the CurrencyType <see cref="Rock.Model.DefinedValue" /> for this transaction.
        /// </value>
        [DataMember]
        [DefinedValue( SystemGuid.DefinedType.FINANCIAL_CURRENCY_TYPE )]
        public int? CurrencyTypeValueId { get; set; }

        /// <summary>
        /// Gets or sets the DefinedValueId of the credit card type <see cref="Rock.Model.DefinedValue"/> indicating the credit card brand/type that was used
        /// to make this transaction. This value will be null for transactions that were not made by credit card.
        /// </summary>
        /// <value>
        /// A <see cref="System.Int32"/> representing the DefinedValueId of the credit card type <see cref="Rock.Model.DefinedValue"/> that was used to make this transaction.
        /// This value value will be null for transactions that were not made by credit card.
        /// </value>
        [DataMember]
        [DefinedValue( SystemGuid.DefinedType.FINANCIAL_CREDIT_CARD_TYPE )]
        public int? CreditCardTypeValueId { get; set; }

        private string _nameOnCardEncrypted = null;
        /// <summary>
        /// Gets or sets the name on card encrypted.
        /// </summary>
        /// <value>
        /// The name on card encrypted.
        /// </value>
        [DataMember]
        [MaxLength( 256 )]
        [Obsolete( "Use NameOnCard" )]
        [RockObsolete( "1.12.4" )]
        public string NameOnCardEncrypted
        {
            get
            {
                // We are only checking null here because empty string is valid.
                if ( _nameOnCard.IsNull() )
                {
                    return _nameOnCardEncrypted;
                }
                return Encryption.EncryptString( _nameOnCard );
            }
            set
            {
                _nameOnCardEncrypted = value;
            }
        }

        private string _nameOnCard = null;
        /// <summary>
        /// Gets the name on card.
        /// </summary>
        /// <value>
        /// The name on card.
        /// </value>
        [DataMember]
        public string NameOnCard
        {
            get
            {
                // We are only checking null here because empty string is valid.
                if ( _nameOnCard.IsNull() && _nameOnCardEncrypted.IsNotNullOrWhiteSpace() )
                {
                    return Encryption.DecryptString( _nameOnCardEncrypted );
                }
                return _nameOnCard;
            }
            set
            {
                _nameOnCard = value;
            }
        }

        private string _expirationMonthEncrypted = null;
        private string _expirationYearEncrypted = null;

        /// <summary>
        /// Gets or sets the expiration month encrypted. Use <seealso cref="ExpirationMonth"/> to get the unencrypted version of Month.
        /// </summary>
        /// <value>
        /// The expiration month encrypted.
        /// </value>
        [DataMember]
        [MaxLength( 256 )]
        [Obsolete( "Use ExpirationMonth" )]
        [RockObsolete( "1.12.4" )]
        public string ExpirationMonthEncrypted
        {
            get
            {
                if ( _expirationMonth != null )
                {
                    return Encryption.EncryptString( _expirationMonth.Value.ToString() );
                }
                return _expirationMonthEncrypted;
            }

            set
            {
                _expirationMonthEncrypted = value;
            }
        }

        /// <summary>
        /// Important Note: that this could be a 2 digit or 4 digit year, so use <seealso cref="ExpirationYear"/> to get the unencrypted version of this which will always return a 4 digit year.
        /// </summary>
        /// <value>
        /// The expiration year encrypted.
        /// </value>
        [DataMember]
        [MaxLength( 256 )]
        [Obsolete( "Use ExpirationYear" )]
        [RockObsolete( "1.12.4" )]
        public string ExpirationYearEncrypted
        {
            get
            {
                if ( _expirationYear != null )
                {
                    return Encryption.EncryptString( _expirationYear.Value.ToString() );
                }
                return _expirationYearEncrypted;
            }
            set
            {
                _expirationYearEncrypted = value;
            }
        }

        private DateTime? _cardExpirationDate = null;

        /// <summary>
        /// Gets the card expiration date.
        /// </summary>
        /// <value>
        /// The card expiration date.
        /// </value>
        [DataMember]
        public DateTime? CardExpirationDate
        {
            get
            {
                var expMonth = ExpirationMonth;
                var expYear = ExpirationYear;
                if ( expMonth.HasValue && expYear.HasValue )
                {
                    return new DateTime( expYear.Value, expMonth.Value, DateTime.DaysInMonth( expYear.Value, expMonth.Value ) );
                }

                return null;
            }
            private set
            {
                _cardExpirationDate = value;
                if ( _cardExpirationDate == null )
                {
                    _expirationMonth = null;
                    _expirationYear = null;
                }
                else
                {
                    _expirationMonth = _cardExpirationDate.Value.Month;
                    _expirationYear = _cardExpirationDate.Value.Year;
                }
            }
        }

        /// <summary>
        /// Gets or sets the billing location identifier.
        /// </summary>
        /// <value>
        /// The billing location identifier.
        /// </value>
        [DataMember]
        public int? BillingLocationId { get; set; }

        /// <summary>
        /// Gets or sets the Gateway Person Identifier.
        /// This would indicate id the customer vault information on the gateway.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the Gateway Person Identifier of the account.
        /// </value>
        [DataMember]
        [MaxLength( 50 )]
        public string GatewayPersonIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rock.Model.FinancialPersonSavedAccount"/> id that was used for this transaction (if there was one)
        /// </summary>
        /// <value>
        /// The financial person saved account.
        /// </value>
        [DataMember]
        [IgnoreCanDelete]
        public int? FinancialPersonSavedAccountId { get; set; }

        #endregion

        #region Virtual Properties

        private int? _expirationMonth = null;
        private int? _expirationYear = null;

        /// <summary>
        /// Gets the expiration month by decrypting ExpirationMonthEncrypted
        /// </summary>
        /// <value>
        /// The expiration month.
        /// </value>
        [DataMember]
        [HideFromReporting]
        public int? ExpirationMonth
        {
            /* MDP 2020-03-13
               NOTE: This is not really a [DataMember] (see <seealso cref="FinancialPaymentDetailConfiguration"/>)
            */

            get
            {
                if ( _expirationMonth == null && _expirationMonthEncrypted != null )
                {
                    return Encryption.DecryptString( _expirationMonthEncrypted ).AsIntegerOrNull();
                }

                return _expirationMonth;
            }
            set
            {
                _expirationMonth = value;
            }
        }

        /// <summary>
        /// Gets the 4 digit year by decrypting ExpirationYearEncrypted and correcting to a 4 digit year if ExpirationYearEncrypted is just a 2 digit year
        /// </summary>
        /// <value>
        /// The expiration year.
        /// </value>
        [DataMember]
        [HideFromReporting]
        public int? ExpirationYear
        {
            /* MDP 2020-03-13
               NOTE: This is not really a [DataMember] (see <seealso cref="FinancialPaymentDetailConfiguration"/>)
            */

            get
            {
                if ( _expirationYear == null && _expirationYearEncrypted != null )
                {
                    return ToFourDigitYear( Encryption.DecryptString( _expirationYearEncrypted ).AsIntegerOrNull() );
                }

                return _expirationYear;
            }

            set
            {
                _expirationYear = ToFourDigitYear( value );
            }
        }

        private int? ToFourDigitYear( int? year )
        {
            if ( year == null || year >= 100 )
            {
                return year;
            }
            else
            {
                return System.Globalization.CultureInfo.CurrentCulture.Calendar.ToFourDigitYear( year.Value );
            }
        }

        /// <summary>
        /// Gets the expiration date formatted as mm/yy, as per ISO7813 https://en.wikipedia.org/wiki/ISO/IEC_7813
        /// </summary>
        /// <value>
        /// The expiration date.
        /// </value>
        [NotMapped]
        public string ExpirationDate
        {
            get
            {
                int? expMonth = ExpirationMonth;
                int? expYear = ExpirationYear;
                if ( expMonth.HasValue && expYear.HasValue )
                {
                    // expYear is 4 digits, but just in case, check if it is 4 digits before just getting the last 2
                    string expireYY = expYear.Value.ToString();
                    if ( expireYY.Length == 4 )
                    {
                        expireYY = expireYY.Substring( 2 );
                    }

                    return $"{expMonth.Value:00}/{expireYY:00}";
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the currency type <see cref="Rock.Model.DefinedValue"/> indicating the type of currency that was used for this
        /// transaction.
        /// </summary>
        /// <value>
        /// A <see cref="Rock.Model.DefinedValue"/> indicating the type of currency that was used for the transaction.
        /// </value>
        [DataMember]
        public virtual DefinedValue CurrencyTypeValue { get; set; }

        /// <summary>
        /// Gets or sets the credit card type <see cref="Rock.Model.DefinedValue"/> indicating the type of credit card that was used for this transaction.
        /// If this was not a credit card based transaction, this value will be null.
        /// </summary>
        /// <value>
        /// A <see cref="Rock.Model.DefinedValue" /> indicating the type of credit card that was used for this transaction. This value is null
        /// for transactions that were not made by credit card.
        /// </value>
        [DataMember]
        public virtual DefinedValue CreditCardTypeValue { get; set; }

        /// <summary>
        /// Gets or sets the billing <see cref="Rock.Model.Location"/>.
        /// </summary>
        /// <value>
        /// The billing location.
        /// </value>
        [DataMember]
        public virtual Location BillingLocation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rock.Model.FinancialPersonSavedAccount"/> that was used for this transaction (if there was one)
        /// </summary>
        /// <value>
        /// The financial person saved account.
        /// </value>
        [DataMember]
        public virtual FinancialPersonSavedAccount FinancialPersonSavedAccount { get; set; }

        /// <summary>
        /// Gets the type of the currency and credit card.
        /// </summary>
        /// <value>
        /// The type of the currency and credit card.
        /// </value>
        [NotMapped]
        public virtual string CurrencyAndCreditCardType
        {
            get
            {
                var sb = new StringBuilder();

                if ( CurrencyTypeValue != null )
                {
                    sb.Append( CurrencyTypeValue.Value );
                }

                if ( CreditCardTypeValue != null )
                {
                    sb.AppendFormat( " - {0}", CreditCardTypeValue.Value );
                }

                return sb.ToString();
            }
        }


        /// <summary>
        /// Gets or sets the history changes.
        /// </summary>
        /// <value>
        /// The history changes.
        /// </value>
        [NotMapped]
        [RockObsolete( "1.8" )]
        [Obsolete( "Use HistoryChangeList", true )]
        public virtual List<string> HistoryChanges { get; set; }

        /// <summary>
        /// Gets or sets the history changes.
        /// </summary>
        /// <value>
        /// The history changes.
        /// </value>
        [NotMapped]
        public virtual History.HistoryChangeList HistoryChangeList { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.AccountNumberMasked;
        }

        /// <summary>
        /// Clears the payment information.
        /// Use this before telling a gateway to update the payment info for an existing transaction.
        /// </summary>
        public void ClearPaymentInfo()
        {
            AccountNumberMasked = null;
            GatewayPersonIdentifier = null;
            FinancialPersonSavedAccountId = null;

            CurrencyTypeValueId = null;
            CreditCardTypeValueId = null;

            NameOnCard = null;
            ExpirationMonth = null;
            ExpirationYear = null;
        }

        /// <summary>
        /// Sets any payment information that the <seealso cref="GatewayComponent">paymentGateway</seealso> didn't set
        /// </summary>
        /// <param name="paymentInfo">The payment information.</param>
        /// <param name="paymentGateway">The payment gateway.</param>
        /// <param name="rockContext">The rock context.</param>
        public void SetFromPaymentInfo( PaymentInfo paymentInfo, GatewayComponent paymentGateway, RockContext rockContext )
        {
            /* 2020-08-27 MDP
             This method should only update values haven't been set yet. So
                1) Make sure paymentInfo has the data (isn't null or whitespace)
                2) Don't overwrite data in this (FinancialPaymentDetail) that already has the data set.
             */

            if ( AccountNumberMasked.IsNullOrWhiteSpace() && paymentInfo.MaskedNumber.IsNotNullOrWhiteSpace() )
            {
                AccountNumberMasked = paymentInfo.MaskedNumber;
            }

            if ( paymentInfo is ReferencePaymentInfo referencePaymentInfo )
            {
                if ( GatewayPersonIdentifier.IsNullOrWhiteSpace() )
                {
                    GatewayPersonIdentifier = referencePaymentInfo.GatewayPersonIdentifier;
                }

                if ( !FinancialPersonSavedAccountId.HasValue )
                {
                    FinancialPersonSavedAccountId = referencePaymentInfo.FinancialPersonSavedAccountId;
                }
            }

            if ( !CurrencyTypeValueId.HasValue && paymentInfo.CurrencyTypeValue != null )
            {
                CurrencyTypeValueId = paymentInfo.CurrencyTypeValue.Id;
            }

            if ( !CreditCardTypeValueId.HasValue && paymentInfo.CreditCardTypeValue != null )
            {
                CreditCardTypeValueId = paymentInfo.CreditCardTypeValue.Id;
            }

            if ( paymentInfo is CreditCardPaymentInfo )
            {
                var ccPaymentInfo = ( CreditCardPaymentInfo ) paymentInfo;

                string nameOnCard = paymentGateway.SplitNameOnCard ? ccPaymentInfo.NameOnCard + " " + ccPaymentInfo.LastNameOnCard : ccPaymentInfo.NameOnCard;

                // since the Address info could coming from an external system (the Gateway), don't do Location validation when creating a new location
                var newLocation = new LocationService( rockContext ).Get(
                    ccPaymentInfo.BillingStreet1, ccPaymentInfo.BillingStreet2, ccPaymentInfo.BillingCity, ccPaymentInfo.BillingState, ccPaymentInfo.BillingPostalCode, ccPaymentInfo.BillingCountry, new GetLocationArgs { ValidateLocation = false, CreateNewLocation = true } );

                if ( NameOnCard.IsNullOrWhiteSpace() && nameOnCard.IsNotNullOrWhiteSpace() )
                {
                    NameOnCard = nameOnCard;
                }

                if ( !ExpirationMonth.HasValue )
                {
                    ExpirationMonth = ccPaymentInfo.ExpirationDate.Month;
                }

                if ( !ExpirationYear.HasValue )
                {
                    ExpirationYear = ccPaymentInfo.ExpirationDate.Year;
                }

                if ( !BillingLocationId.HasValue && newLocation != null )
                {
                    BillingLocationId = newLocation.Id;
                }
            }
            else if ( paymentInfo is SwipePaymentInfo )
            {
                var swipePaymentInfo = ( SwipePaymentInfo ) paymentInfo;

                if ( NameOnCard.IsNullOrWhiteSpace() && swipePaymentInfo.NameOnCard.IsNotNullOrWhiteSpace() )
                {
                    NameOnCard = swipePaymentInfo.NameOnCard;
                }

                if ( !ExpirationMonth.HasValue )
                {
                    ExpirationMonth = swipePaymentInfo.ExpirationDate.Month;
                }

                if ( !ExpirationYear.HasValue )
                {
                    ExpirationYear = swipePaymentInfo.ExpirationDate.Year;
                }
            }
            else
            {
                // since the Address info could coming from an external system (the Gateway), don't do Location validation when creating a new location
                var newLocation = new LocationService( rockContext ).Get(
                    paymentInfo.Street1, paymentInfo.Street2, paymentInfo.City, paymentInfo.State, paymentInfo.PostalCode, paymentInfo.Country, new GetLocationArgs { ValidateLocation = false, CreateNewLocation = true } );

                if ( !BillingLocationId.HasValue && newLocation != null )
                {
                    BillingLocationId = newLocation.Id;
                }

            }
        }

        /// <summary>
        /// Method that will be called on an entity immediately before the item is saved by context
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="entry"></param>
        public override void PreSaveChanges( Rock.Data.DbContext dbContext, DbEntityEntry entry )
        {
            var rockContext = ( RockContext ) dbContext;
            HistoryChangeList = new History.HistoryChangeList();

            switch ( entry.State )
            {
                case EntityState.Added:
                    {
                        History.EvaluateChange( HistoryChangeList, "Account Number", string.Empty, AccountNumberMasked );
                        History.EvaluateChange( HistoryChangeList, "Currency Type", ( int? ) null, CurrencyTypeValue, CurrencyTypeValueId );
                        History.EvaluateChange( HistoryChangeList, "Credit Card Type", ( int? ) null, CreditCardTypeValue, CreditCardTypeValueId );
                        History.EvaluateChange( HistoryChangeList, "Name On Card", string.Empty, AccountNumberMasked, true );
                        History.EvaluateChange( HistoryChangeList, "Expiration Month", string.Empty, ExpirationMonth.ToStringSafe(), true );
                        History.EvaluateChange( HistoryChangeList, "Expiration Year", string.Empty, ExpirationYear.ToStringSafe(), true );
                        History.EvaluateChange( HistoryChangeList, "Billing Location", string.Empty, History.GetValue<Location>( BillingLocation, BillingLocationId, rockContext ) );
                        break;
                    }
                case EntityState.Modified:
                case EntityState.Deleted:
                    {
                        History.EvaluateChange( HistoryChangeList, "Account Number", entry.OriginalValues["AccountNumberMasked"].ToStringSafe(), AccountNumberMasked );
                        History.EvaluateChange( HistoryChangeList, "Currency Type", entry.OriginalValues["CurrencyTypeValueId"].ToStringSafe().AsIntegerOrNull(), CurrencyTypeValue, CurrencyTypeValueId );
                        History.EvaluateChange( HistoryChangeList, "Credit Card Type", entry.OriginalValues["CreditCardTypeValueId"].ToStringSafe().AsIntegerOrNull(), CreditCardTypeValue, CreditCardTypeValueId );
                        History.EvaluateChange( HistoryChangeList, "Name On Card", entry.OriginalValues["AccountNumberMasked"].ToStringSafe(), AccountNumberMasked, true );
                        History.EvaluateChange( HistoryChangeList, "Expiration Month", entry.OriginalValues["ExpirationMonth"].ToStringSafe(), ExpirationMonth.ToStringSafe(), true );
                        History.EvaluateChange( HistoryChangeList, "Expiration Year", entry.OriginalValues["ExpirationYear"].ToStringSafe(), ExpirationYear.ToStringSafe(), true );
                        History.EvaluateChange( HistoryChangeList, "Billing Location", History.GetValue<Location>( null, entry.OriginalValues["BillingLocationId"].ToStringSafe().AsIntegerOrNull(), rockContext ), History.GetValue<Location>( BillingLocation, BillingLocationId, rockContext ) );
                        break;
                    }
            }

            if ( entry.State == EntityState.Added || entry.State == EntityState.Modified )
            {
                // Ensure that CurrencyTypeValueId is set. The UI tries to prevent it, but just in case, if it isn't, set it to Unknown
                if ( !this.CurrencyTypeValueId.HasValue )
                {
                    this.CurrencyTypeValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_UNKNOWN.AsGuid() )?.Id;
                }
            }

            base.PreSaveChanges( dbContext, entry );
        }

        /// <summary>
        /// Method that will be called on an entity immediately after the item is saved by context
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public override void PostSaveChanges( Data.DbContext dbContext )
        {
            if ( HistoryChangeList?.Any() == true )
            {
                foreach ( var txn in new FinancialTransactionService( ( RockContext ) dbContext )
                    .Queryable().AsNoTracking()
                    .Where( t => t.FinancialPaymentDetailId == this.Id )
                    .Select( t => new { t.Id, t.BatchId } )
                    .ToList() )
                {
                    HistoryService.SaveChanges( ( RockContext ) dbContext, typeof( FinancialTransaction ), Rock.SystemGuid.Category.HISTORY_FINANCIAL_TRANSACTION.AsGuid(), txn.Id, HistoryChangeList, true, this.ModifiedByPersonAliasId, dbContext.SourceOfChange );
                    var batchHistory = new History.HistoryChangeList();
                    batchHistory.AddChange( History.HistoryVerb.Modify, History.HistoryChangeType.Property, "Transaction" );
                    HistoryService.SaveChanges( ( RockContext ) dbContext, typeof( FinancialBatch ), Rock.SystemGuid.Category.HISTORY_FINANCIAL_TRANSACTION.AsGuid(), txn.BatchId.Value, batchHistory, string.Empty, typeof( FinancialTransaction ), txn.Id, true, this.ModifiedByPersonAliasId, dbContext.SourceOfChange );
                }
            }

            base.PostSaveChanges( dbContext );
        }

        #endregion
    }

    #region Entity Configuration

    /// <summary>
    /// FinancialPersonBankAccount Configuration class.
    /// </summary>
    public partial class FinancialPaymentDetailConfiguration : EntityTypeConfiguration<FinancialPaymentDetail>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinancialPersonBankAccountConfiguration"/> class.
        /// </summary>
        public FinancialPaymentDetailConfiguration()
        {
            this.HasOptional( t => t.CurrencyTypeValue ).WithMany().HasForeignKey( t => t.CurrencyTypeValueId ).WillCascadeOnDelete( false );
            this.HasOptional( t => t.CreditCardTypeValue ).WithMany().HasForeignKey( t => t.CreditCardTypeValueId ).WillCascadeOnDelete( false );
            this.HasOptional( t => t.BillingLocation ).WithMany().HasForeignKey( t => t.BillingLocationId ).WillCascadeOnDelete( false );

            /*
             * 2020-06-12 - JH
             *
             * When a FinancialPersonSavedAccount record that this FinancialPaymentDetail references is deleted, SQL will simply null-out the
             * FinancialPaymentDetail.FinancialPersonSavedAccountId field. See here for how we manually introduced this "ON DELETE SET NULL"
             * behavior:
             *
             * https://github.com/SparkDevNetwork/Rock/commit/6953aa1986d46c9c84663ce818333425c0807c01#diff-e0c4fac8254b21998bb9235c3dee4ee9R36
             */
            this.HasOptional( t => t.FinancialPersonSavedAccount ).WithMany().HasForeignKey( t => t.FinancialPersonSavedAccountId ).WillCascadeOnDelete( true );
        }
    }

    #endregion
}