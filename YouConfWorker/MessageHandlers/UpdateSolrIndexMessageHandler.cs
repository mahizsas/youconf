﻿
using SolrNet;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouConf.Common.Data;
using YouConf.Common.Messaging;
using YouConfWorker.Data.SolrEntities;

namespace YouConfWorker.MessageHandlers
{
    public class UpdateSolrIndexMessageHandler : IMessageHandler<UpdateSolrIndexMessage>
    {
        public IYouConfDbContext Db { get; set; }
        public ISolrOperations<ConferenceDto> Solr { get; set; }

        public UpdateSolrIndexMessageHandler(IYouConfDbContext db, ISolrOperations<ConferenceDto> solr)
        {
            Db = db;
            Solr = solr;
        }

        public void Handle(UpdateSolrIndexMessage message)
        {
            if (message.Action == SolrIndexAction.Delete)
            {
                Solr.Delete(message.ConferenceId.ToString());
            }
            else
            {
                var conference = Db.Conferences.First(x => x.Id == message.ConferenceId);
                Solr.Add(new ConferenceDto()
                {
                    ID = conference.Id,
                    HashTag = conference.HashTag,
                    Title = conference.Name,
                    Content = conference.Abstract + " " + conference.Description,
                    Speakers = conference.Speakers
                        .Select(x => x.Name)
                        .ToList()
                });
            }
            Solr.Commit();
        }
    }
}